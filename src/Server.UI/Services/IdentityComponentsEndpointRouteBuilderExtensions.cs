using System.Security.Claims;
using System.Text.Json;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;
using CleanArchitecture.Blazor.Server.UI.Pages.Identity.Login;
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
    public static readonly string ExternalLogin = "/pages/authentication/externallogin";
    public static readonly string Logout = "/pages/authentication/logout";
    public static readonly string Login = "/pages/authentication/login";
    public static readonly string TwofaVerify = "/pages/authentication/2fa/verify";
    public static readonly string TwofaRecovery = "/pages/authentication/2fa/recovery";
    public static readonly string PerformLinkExternalLogin = "/pages/authentication/performlinkexternallogin";

    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IEndpointConventionBuilder");

        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/pages/authentication");

        accountGroup.MapGet("login", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromQuery] string userName,
            [FromQuery] string password,
            [FromQuery] bool rememberMe = false,
            [FromQuery] string? returnUrl = null) =>
        {
            try
            {
                // Security check: Only allow requests from the same origin
                var referer = context.Request.Headers.Referer.ToString();
                var host = context.Request.Host.ToString();
                var scheme = context.Request.Scheme;
                var expectedOrigin = $"{scheme}://{host}";

                if (string.IsNullOrEmpty(referer) || !referer.StartsWith(expectedOrigin, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning("Login attempt from unauthorized origin. Referer: {Referer}, Expected: {Expected}", referer, expectedOrigin);
                    return Results.Forbid();
                }
                // Validate parameters
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return Results.BadRequest("Username and password are required");
                }
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
                var checkResult = await signInManager.PasswordSignInAsync(user, password, true, true);
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
                        //var message="Your account is not allowed to log in. Please ensure your account has been activated and you have completed all required steps";
                        return Results.Redirect("/account/invaliduser");
                    }
                    else
                    {
                        return Results.Redirect("/account/invaliduser");
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
        });

        accountGroup.MapGet("2fa/verify", async (HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string token,
            [FromQuery] bool remember,
            [FromQuery] string? returnUrl = null
            ) =>
        {
            // Security check: Only allow requests from the same origin
            var referer = context.Request.Headers.Referer.ToString();
            var host = context.Request.Host.ToString();
            var scheme = context.Request.Scheme;
            var expectedOrigin = $"{scheme}://{host}";

            if (string.IsNullOrEmpty(referer) || !referer.StartsWith(expectedOrigin, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Login attempt from unauthorized origin. Referer: {Referer}, Expected: {Expected}", referer, expectedOrigin);
                return Results.Forbid();
            }
            // Validate parameters
            if (string.IsNullOrEmpty(token))
            {
                return Results.BadRequest("Token is required");
            }

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Results.NotFound();
            }
            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(token,
                                  remember, remember);
            if (result.Succeeded)
            {
                return Results.Redirect("/");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    return Results.Redirect("/account/lockout");
                }
                else if (result.IsNotAllowed)
                {
                    return Results.BadRequest("Your account is not allowed to log in. Please ensure your account has been activated and you have completed all required steps");
                }
                else
                {
                    return Results.BadRequest("Invalid token");
                }
            }

        });

        accountGroup.MapGet("2fa/recovery", async (HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string code,
            [FromQuery] string? returnUrl = null) =>
        {
            // Security check: Only allow requests from the same origin
            var referer = context.Request.Headers.Referer.ToString();
            var host = context.Request.Host.ToString();
            var scheme = context.Request.Scheme;
            var expectedOrigin = $"{scheme}://{host}";

            if (string.IsNullOrEmpty(referer) || !referer.StartsWith(expectedOrigin, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Login attempt from unauthorized origin. Referer: {Referer}, Expected: {Expected}", referer, expectedOrigin);
                return Results.Forbid();
            }

            // Validate parameters
            if (string.IsNullOrEmpty(code))
            {
                return Results.BadRequest("Code is required");
            }

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Results.NotFound();
            }

            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(code);
            if (result.Succeeded)
            {
                return Results.Redirect("/");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    return Results.Redirect("/account/lockout");
                }
                else if (result.IsNotAllowed)
                {
                    return Results.BadRequest("Your account is not allowed to log in. Please ensure your account has been activated and you have completed all required steps");
                }
                else
                {
                    return Results.BadRequest("Invalid code");
                }
            }
        });

        accountGroup.MapPost("/performexternallogin", (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            IEnumerable<KeyValuePair<string, StringValues>> query =
            [
                new KeyValuePair<string, StringValues>("ReturnUrl", returnUrl),
                new KeyValuePair<string, StringValues>("Action", LinkExternalLogin.LoginCallbackAction)
            ];

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                ExternalLogin,
                QueryString.Create(query));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            logger.LogInformation("Redirecting to external login provider {Provider} with return URL {ReturnUrl}", provider, returnUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        accountGroup.MapGet("externallogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string action,
            [FromQuery] string? remoteError,
            [FromQuery] string? returnUrl) =>
        {

            if (!string.IsNullOrEmpty(remoteError))
            {
                logger.LogWarning("External login error: {RemoteError}", remoteError);
                return Results.BadRequest($"External login error: {remoteError}");
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                logger.LogWarning("No external login info found.");
                return Results.BadRequest("No external login info found.");
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var provider = info.LoginProvider;
            if (action == "LoginCallback")
            {
                var result = await signInManager.ExternalLoginSignInAsync(
                            info.LoginProvider,
                            info.ProviderKey,
                            false,
                            false);
                if (result.Succeeded)
                {
                    logger.LogInformation("User {UserName} logged in with external provider {Provider}.", info.Principal.Identity?.Name, info.LoginProvider);
                    return Results.Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    logger.LogWarning("User {UserName} is locked out.", info.Principal.Identity?.Name);
                    return Results.Redirect("/account/lockout");
                }
                else if (result.IsNotAllowed)
                {
                    logger.LogWarning("User {UserName} is not allowed to log in.", info.Principal.Identity?.Name);
                    return Results.Redirect("/account/invaliduser");
                }
            }
            IEnumerable<KeyValuePair<string, StringValues>> query =
            [
                new KeyValuePair<string, StringValues>("ReturnUrl", returnUrl),
                new KeyValuePair<string, StringValues>("logincallback", action),
                new KeyValuePair<string, StringValues>("email", email),
                new KeyValuePair<string, StringValues>("provider", provider),
            ];

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                LinkExternalLogin.PageUrl,
                QueryString.Create(query));
            return Results.Redirect(redirectUrl);
        });
        accountGroup.MapGet("/performlinkexternallogin", async (
           HttpContext context,
           [FromServices] SignInManager<ApplicationUser> signInManager,
           [FromServices] RoleManager<ApplicationRole> roleManager,
           [FromServices] UserManager<ApplicationUser> userManager,
           [FromQuery] string action,
           [FromQuery] string tenantId,
           [FromQuery] string timezoneId,
           [FromQuery] string languageCode) =>
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                logger.LogWarning("No external login info found for linking.");
                return Results.BadRequest("No external login info found for linking.");
            }
            var user = new ApplicationUser()
            {
                TenantId = tenantId,
                TimeZoneId = timezoneId,
                LanguageCode = languageCode,
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email) ?? info.Principal.Identity?.Name,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? info.Principal.Identity?.Name,
                DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? info.Principal.Identity?.Name,
                Provider = info.LoginProvider,
                IsActive = true,
                EmailConfirmed = true,
            };
            var role = await roleManager.Roles.Where(x => x.TenantId == user.TenantId && x.Name == RoleName.Basic).FirstOrDefaultAsync();
            if (role is null)
            {
                role = new ApplicationRole
                {
                    Name = RoleName.Basic,
                    NormalizedName = RoleName.Basic.ToUpperInvariant(),
                    TenantId = user.TenantId
                };
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to create role {RoleName} for tenant {TenantId}", RoleName.Basic, user.TenantId);
                    return Results.BadRequest("Failed to create role for tenant.");
                }
            }
            var userResult = await userManager.CreateAsync(user);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to create user {UserName} for tenant {TenantId}", user.UserName, user.TenantId);
                return Results.BadRequest("Failed to create user.");
            }
            userResult = await userManager.AddToRoleAsync(user, role.Name!);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to add user {UserName} to role {RoleName} for tenant {TenantId}", user.UserName, role.Name, user.TenantId);
                return Results.BadRequest("Failed to add user to role.");
            }
            userResult = await userManager.AddLoginAsync(user, info);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to add external login for user {UserName} with provider {Provider}", user.UserName, info.LoginProvider);
                return Results.BadRequest("Failed to add external login.");
            }
            if (action == "LoginCallback")
            {
                await signInManager.SignInAsync(user, true);
                logger.LogInformation("User {UserName} linked external account {Provider} successfully.", user.UserName, info.LoginProvider);
                return Results.Redirect("/");
            }
            else
            {
                return Results.Redirect("/account/login");
            }

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

        var manageGroup = accountGroup.MapGroup("/manage").RequireAuthorization();

       



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