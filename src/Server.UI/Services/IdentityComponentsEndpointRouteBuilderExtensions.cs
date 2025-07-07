using System.Security.Claims;
using System.Text.Json;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Server.UI.Pages.Identity.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Server.UI.Services;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    public static readonly string PerformExternalLogin = "/pages/authentication/performexternallogin";

    public static readonly string Logout = "/pages/authentication/logout";
    public static readonly string Login = "/pages/authentication/login";

    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IEndpointConventionBuilder");

        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/pages/authentication");

        accountGroup.MapPost("login", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromForm] string userName,
            [FromForm] string password,
            [FromForm] bool rememberMe,
            [FromForm] string? returnUrl) =>
        {
            try
            {
                // Check if the user exists
                var user = await userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return Results.BadRequest("User does not exist");
                }
                
                if (!user.IsActive)
                {
                    return Results.BadRequest("Your account is inactive. Please contact support");
                }

                // Check password
                var checkResult = await signInManager.CheckPasswordSignInAsync(user, password, true);
                if (!checkResult.Succeeded)
                {
                    if (checkResult.RequiresTwoFactor)
                    {
                        // For two-factor authentication, redirect to 2FA page
                        var safeReturnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                        return Results.Redirect($"/account/loginwith2fa?returnUrl={Uri.EscapeDataString(safeReturnUrl)}&rememberMe={rememberMe}");
                    }
                    else if (checkResult.IsLockedOut)
                    {
                        return Results.Redirect("/account/lockout");
                    }
                    else if (checkResult.IsNotAllowed)
                    {
                        return Results.BadRequest("Your account is not allowed to log in. Please ensure your account has been activated and you have completed all required steps");
                    }
                    else
                    {
                        return Results.BadRequest("Invalid login attempt");
                    }
                }

                // Perform actual sign in
                await signInManager.SignInAsync(user, rememberMe);
                logger.LogInformation("{UserName} has logged in successfully.", userName);
                
                // Redirect to return URL or home page
                var redirectUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return Results.Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login for user {UserName}", userName);
                return Results.StatusCode(500);
            }
        }).DisableAntiforgery();

        accountGroup.MapPost("/performexternallogin", (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            IEnumerable<KeyValuePair<string, StringValues>> query =
            [
                new KeyValuePair<string, StringValues>("ReturnUrl", returnUrl),
                new KeyValuePair<string, StringValues>("Action", ExternalLogin.LoginCallbackAction)
            ];

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                ExternalLogin.PageUrl,
                QueryString.Create(query));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            logger.LogInformation("Redirecting to external login provider {Provider} with return URL {ReturnUrl}", provider, returnUrl);
            return TypedResults.Challenge(properties, [provider]);
        });


        accountGroup.MapPost("/logout", async (
            ClaimsPrincipal user,
            SignInManager<ApplicationUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync().ConfigureAwait(false);
            logger.LogInformation("{UserName} has logged out.", user.Identity?.Name);
            return TypedResults.LocalRedirect($"{returnUrl}");
        }).RequireAuthorization().DisableAntiforgery();

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        manageGroup.MapPost("/LinkExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider) =>
        {
            // Clear the existing external cookie to ensure a clean login process
            await context.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                ExternalLogins.PageUrl,
                QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl,
                signInManager.UserManager.GetUserId(context.User));
            logger.LogInformation("{UserName} is linking external login provider {Provider} with redirect URL {RedirectUrl}", context.User.Identity?.Name, provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

 

        manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
        {
            var user = await userManager.GetUserAsync(context.User).ConfigureAwait(false);
            if (user is null)
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");

            var userId = await userManager.GetUserIdAsync(user).ConfigureAwait(false);
            logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            // Only include personal data for download
            var personalData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps) personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");

            var logins = await userManager.GetLoginsAsync(user).ConfigureAwait(false);
            foreach (var l in logins) personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);

            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false))!);
            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return TypedResults.File(fileBytes, "application/json", "PersonalData.json");
        });


        return accountGroup;
    }
}

public class LoginRequest
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; } = true;
}