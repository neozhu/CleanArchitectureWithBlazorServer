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

    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IEndpointConventionBuilder");

        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/pages/authentication");

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
        });

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