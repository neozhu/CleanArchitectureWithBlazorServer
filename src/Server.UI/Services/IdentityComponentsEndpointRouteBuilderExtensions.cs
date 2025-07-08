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

/// <summary>
/// Provides extension methods for configuring Identity-related endpoints in the application.
/// This class contains all the necessary endpoints for authentication, authorization, and user management operations.
/// </summary>
internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    /// <summary>
    /// The endpoint URL for performing external login operations.
    /// </summary>
    public static readonly string PerformExternalLogin = "/pages/authentication/performexternallogin";
    
    /// <summary>
    /// The endpoint URL for handling external login callbacks.
    /// </summary>
    public static readonly string ExternalLogin = "/pages/authentication/externallogin";
    
    /// <summary>
    /// The endpoint URL for user logout operations.
    /// </summary>
    public static readonly string Logout = "/pages/authentication/logout";
    
    /// <summary>
    /// The endpoint URL for user login operations.
    /// </summary>
    public static readonly string Login = "/pages/authentication/login";
    
    /// <summary>
    /// The endpoint URL for two-factor authentication verification.
    /// </summary>
    public static readonly string TwofaVerify = "/pages/authentication/2fa/verify";
    
    /// <summary>
    /// The endpoint URL for two-factor authentication recovery.
    /// </summary>
    public static readonly string TwofaRecovery = "/pages/authentication/2fa/recovery";
    
    /// <summary>
    /// The endpoint URL for performing external login linking operations.
    /// </summary>
    public static readonly string PerformLinkExternalLogin = "/pages/authentication/performlinkexternallogin";

    /// <summary>
    /// Maps additional Identity endpoints required by the Identity Razor components.
    /// These endpoints handle authentication, authorization, and user management operations.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to add routes to.</param>
    /// <returns>An <see cref="IEndpointConventionBuilder"/> for further configuration.</returns>
    /// <exception cref="ArgumentNullException">Thrown when endpoints parameter is null.</exception>
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IEndpointConventionBuilder");

        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/pages/authentication");

        // Configure login endpoint with credential-based authentication
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
                // Security validation: Ensure request originates from the same domain to prevent CSRF attacks
                var referer = context.Request.Headers.Referer.ToString();
                var host = context.Request.Host.ToString();
                var scheme = context.Request.Scheme;
                var expectedOrigin = $"{scheme}://{host}";

                if (string.IsNullOrEmpty(referer) || !referer.StartsWith(expectedOrigin, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning("Login attempt from unauthorized origin. Referer: {Referer}, Expected: {Expected}", referer, expectedOrigin);
                    return Results.Forbid();
                }
                
                // Validate required authentication parameters
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return Results.BadRequest("Username and password are required");
                }
                
                // Verify user existence in the system
                var user = await userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return Results.BadRequest("User does not exist");
                }

                // Check if user account is active
                if (!user.IsActive)
                {
                    return Results.BadRequest("Your account is inactive. Please contact support");
                }

                // Attempt password-based sign-in with lockout protection
                var checkResult = await signInManager.PasswordSignInAsync(user, password, true, true);
                if (!checkResult.Succeeded)
                {
                    if (checkResult.RequiresTwoFactor)
                    {
                        // Redirect to two-factor authentication page
                        var safeReturnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                        return Results.Redirect($"/account/loginwith2fa?returnUrl={Uri.EscapeDataString(safeReturnUrl)}&rememberMe={rememberMe}");
                    }
                    else if (checkResult.IsLockedOut)
                    {
                        return Results.Redirect("/account/lockout");
                    }
                    else if (checkResult.IsNotAllowed)
                    {
                        // Account exists but is not allowed to sign in (e.g., email not confirmed)
                        return Results.Redirect("/account/invaliduser");
                    }
                    else
                    {
                        return Results.Redirect("/account/invaliduser");
                    }
                }

                // Successful authentication - create authentication session
                await signInManager.SignInAsync(user, rememberMe);
                logger.LogInformation("{UserName} has logged in successfully.", userName);

                // Redirect to the originally requested page or home
                var redirectUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return Results.Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login for user {UserName}", userName);
                return Results.StatusCode(500);
            }
        });

        // Configure two-factor authentication verification endpoint
        accountGroup.MapGet("2fa/verify", async (HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string token,
            [FromQuery] bool remember,
            [FromQuery] string? returnUrl = null
            ) =>
        {
            // Origin validation for security
            var referer = context.Request.Headers.Referer.ToString();
            var host = context.Request.Host.ToString();
            var scheme = context.Request.Scheme;
            var expectedOrigin = $"{scheme}://{host}";

            if (string.IsNullOrEmpty(referer) || !referer.StartsWith(expectedOrigin, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Login attempt from unauthorized origin. Referer: {Referer}, Expected: {Expected}", referer, expectedOrigin);
                return Results.Forbid();
            }
            
            // Validate the 2FA token parameter
            if (string.IsNullOrEmpty(token))
            {
                return Results.BadRequest("Token is required");
            }

            // Retrieve the user who initiated 2FA process
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Results.NotFound();
            }
            
            // Verify the authenticator token
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

        // Configure two-factor authentication recovery code endpoint
        accountGroup.MapGet("2fa/recovery", async (HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string code,
            [FromQuery] string? returnUrl = null) =>
        {
            // Security validation for request origin
            var referer = context.Request.Headers.Referer.ToString();
            var host = context.Request.Host.ToString();
            var scheme = context.Request.Scheme;
            var expectedOrigin = $"{scheme}://{host}";

            if (string.IsNullOrEmpty(referer) || !referer.StartsWith(expectedOrigin, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Login attempt from unauthorized origin. Referer: {Referer}, Expected: {Expected}", referer, expectedOrigin);
                return Results.Forbid();
            }

            // Validate recovery code parameter
            if (string.IsNullOrEmpty(code))
            {
                return Results.BadRequest("Code is required");
            }

            // Get user from 2FA session
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Results.NotFound();
            }

            // Attempt sign-in with recovery code
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

        // Configure external login initiation endpoint
        accountGroup.MapPost("/performexternallogin", (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            // Build query parameters for external login callback
            IEnumerable<KeyValuePair<string, StringValues>> query =
            [
                new KeyValuePair<string, StringValues>("ReturnUrl", returnUrl),
                new KeyValuePair<string, StringValues>("Action", LinkExternalLogin.LoginCallbackAction)
            ];

            // Construct the callback URL for the external provider
            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                ExternalLogin,
                QueryString.Create(query));

            // Configure authentication properties for the external provider
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            logger.LogInformation("Redirecting to external login provider {Provider} with return URL {ReturnUrl}", provider, returnUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        // Configure external login callback handling endpoint
        accountGroup.MapGet("externallogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string action,
            [FromQuery] string? remoteError,
            [FromQuery] string? returnUrl) =>
        {
            // Handle errors from external provider
            if (!string.IsNullOrEmpty(remoteError))
            {
                logger.LogWarning("External login error: {RemoteError}", remoteError);
                return Results.BadRequest($"External login error: {remoteError}");
            }
            
            // Retrieve external login information from the authentication context
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                logger.LogWarning("No external login info found.");
                return Results.BadRequest("No external login info found.");
            }
            
            // Extract user information from external provider claims
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var provider = info.LoginProvider;
            
            if (action == "LoginCallback")
            {
                // Attempt to sign in using external login information
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
            
            // Build query parameters for external login linking page
            IEnumerable<KeyValuePair<string, StringValues>> query =
            [
                new KeyValuePair<string, StringValues>("ReturnUrl", returnUrl),
                new KeyValuePair<string, StringValues>("logincallback", action),
                new KeyValuePair<string, StringValues>("email", email),
                new KeyValuePair<string, StringValues>("provider", provider),
            ];

            // Redirect to external login linking page for account association
            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                LinkExternalLogin.PageUrl,
                QueryString.Create(query));
            return Results.Redirect(redirectUrl);
        });
        
        // Configure external login account linking endpoint
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
            // Retrieve external login information for account linking
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                logger.LogWarning("No external login info found for linking.");
                return Results.BadRequest("No external login info found for linking.");
            }
            
            // Create new user account with external provider information
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
            
            // Ensure the basic role exists for the tenant
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
            
            // Create the user account
            var userResult = await userManager.CreateAsync(user);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to create user {UserName} for tenant {TenantId}", user.UserName, user.TenantId);
                return Results.BadRequest("Failed to create user.");
            }
            
            // Assign the user to the basic role
            userResult = await userManager.AddToRoleAsync(user, role.Name!);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to add user {UserName} to role {RoleName} for tenant {TenantId}", user.UserName, role.Name, user.TenantId);
                return Results.BadRequest("Failed to add user to role.");
            }
            
            // Link the external login to the user account
            userResult = await userManager.AddLoginAsync(user, info);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to add external login for user {UserName} with provider {Provider}", user.UserName, info.LoginProvider);
                return Results.BadRequest("Failed to add external login.");
            }
            
            if (action == "LoginCallback")
            {
                // Sign in the newly created user
                await signInManager.SignInAsync(user, true);
                logger.LogInformation("User {UserName} linked external account {Provider} successfully.", user.UserName, info.LoginProvider);
                return Results.Redirect("/");
            }
            else
            {
                return Results.Redirect("/account/login");
            }

        });
        
        // Configure user logout endpoint
        accountGroup.MapPost("/logout", async (
            ClaimsPrincipal user,
            SignInManager<ApplicationUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            // Sign out the current user and clear authentication cookies
            await signInManager.SignOutAsync().ConfigureAwait(false);
            logger.LogInformation("{UserName} has logged out.", user.Identity?.Name);
            return TypedResults.LocalRedirect($"{returnUrl}");
        }).RequireAuthorization().DisableAntiforgery();

        // Create a group for user management endpoints (requires authentication)
        var manageGroup = accountGroup.MapGroup("/manage").RequireAuthorization();

        // Configure personal data download endpoint
        manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
        {
            // Get the current authenticated user
            var user = await userManager.GetUserAsync(context.User).ConfigureAwait(false);
            if (user is null)
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");

            var userId = await userManager.GetUserIdAsync(user).ConfigureAwait(false);
            logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            // Collect personal data marked with PersonalDataAttribute
            var personalData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps) personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");

            // Include external login provider information
            var logins = await userManager.GetLoginsAsync(user).ConfigureAwait(false);
            foreach (var l in logins) personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);

            // Include authenticator key if available
            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false))!);
            
            // Serialize personal data to JSON format
            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            // Set response headers for file download
            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return TypedResults.File(fileBytes, "application/json", "PersonalData.json");
        });

        return accountGroup;
    }
}
