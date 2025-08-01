using System.Security.Claims;
using System.Text.Json;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Application.Common.Constants.Roles;
using CleanArchitecture.Blazor.Server.UI.Pages.Identity.Login;
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

    // Redirect URLs for various authentication scenarios
    private static class RedirectUrls
    {
        public const string Home = "/";
        public const string Login = "/account/login";
        public const string Lockout = "/account/lockout";
        public const string InvalidUser = "/account/invaliduser";
        public const string LoginWith2Fa = "/account/loginwith2fa";
    }

    /// <summary>
    /// Validates that the request originates from the same domain to prevent CSRF attacks.
    /// </summary>
    /// <param name="context">The HTTP context of the request.</param>
    /// <param name="logger">Logger for security warnings.</param>
    /// <returns>True if the origin is valid, false otherwise.</returns>
    private static bool ValidateRequestOrigin(HttpContext context, ILogger logger)
    {
        var referer = context.Request.Headers.Referer.ToString();
        var host = context.Request.Host.ToString();
        var scheme = context.Request.Scheme;
        var expectedOrigin = $"{scheme}://{host}";

        if (string.IsNullOrEmpty(referer) || !referer.StartsWith(expectedOrigin, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogError("Request from unauthorized origin. ");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Handles common sign-in result scenarios and returns appropriate responses.
    /// </summary>
    /// <param name="result">The sign-in result to handle.</param>
    /// <param name="returnUrl">The URL to redirect to on success.</param>
    /// <param name="rememberMe">Whether to remember the user for 2FA.</param>
    /// <returns>An IResult representing the appropriate response.</returns>
    private static Microsoft.AspNetCore.Http.IResult HandleSignInResult(Microsoft.AspNetCore.Identity.SignInResult result, string? returnUrl, bool rememberMe = false)
    {
        if (result.Succeeded)
        {
            var decodedUrl = Uri.UnescapeDataString(returnUrl ?? "");

            if (string.IsNullOrEmpty(decodedUrl) || !decodedUrl.StartsWith("/"))
            {
                decodedUrl = RedirectUrls.Home;
            }
            return Results.Redirect(decodedUrl);
        }
        
        if (result.RequiresTwoFactor)
        {
            var safeReturnUrl = string.IsNullOrEmpty(returnUrl) ? RedirectUrls.Home : returnUrl;
            return Results.Redirect($"{RedirectUrls.LoginWith2Fa}?returnUrl={Uri.EscapeDataString(safeReturnUrl)}&rememberMe={rememberMe}");
        }
        
        if (result.IsLockedOut)
        {
            return Results.Redirect(RedirectUrls.Lockout);
        }
        
        if (result.IsNotAllowed)
        {
            return Results.Redirect(RedirectUrls.InvalidUser);
        }
        
        return Results.Redirect(RedirectUrls.InvalidUser);
    }

    

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
        accountGroup.MapPost("login", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromForm] string? userName=null,
            [FromForm] string? password=null,
            [FromForm] bool rememberMe = false,
            [FromForm] string? returnUrl = null) =>
        {
            try
            {
                // Security validation: Ensure request originates from the same domain to prevent CSRF attacks
                if (!ValidateRequestOrigin(context, logger))
                {
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
                if (checkResult.Succeeded)
                {
                    // Successful authentication - create authentication session
                    //await signInManager.SignInAsync(user, rememberMe);
                    logger.LogInformation("{UserId} has logged in successfully.", user.Id);
                }

                return HandleSignInResult(checkResult, returnUrl, rememberMe);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login for user {userName}", userName);
                return Results.StatusCode(500);
            }
        });

        // Configure two-factor authentication verification endpoint
        accountGroup.MapGet("2fa/verify", async (HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string? token=null,
            [FromQuery] bool remember=false,
            [FromQuery] string? returnUrl = null
            ) =>
        {
            // Origin validation for security
            if (!ValidateRequestOrigin(context, logger))
            {
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
            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(token, remember, remember);
            return HandleSignInResult(result, returnUrl);
        });

        // Configure two-factor authentication recovery code endpoint
        accountGroup.MapGet("2fa/recovery", async (HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromQuery] string? recoveryCode=null,
            [FromQuery] string? returnUrl = null) =>
        {
            // Security validation for request origin
            if (!ValidateRequestOrigin(context, logger))
            {
                return Results.Forbid();
            }

            // Validate recovery code parameter
            if (string.IsNullOrEmpty(recoveryCode))
            {
                return Results.BadRequest("Recovery code is required");
            }

            // Get user from 2FA session
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Results.NotFound();
            }

            // Attempt sign-in with recovery code
            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            return HandleSignInResult(result, returnUrl);
        });

        // Configure external login initiation endpoint
        accountGroup.MapPost("/performexternallogin", (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string? provider=null,
            [FromForm] string? returnUrl=null) =>
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
            [FromQuery] string? action=null,
            [FromQuery] string? remoteError=null,
            [FromQuery] string? returnUrl=null) =>
        {
            // Handle errors from external provider
            if (!string.IsNullOrEmpty(remoteError))
            {
                logger.LogError("External login error: {RemoteError}", remoteError);
                return Results.BadRequest($"External login error: {remoteError}");
            }
        
            // Retrieve external login information from the authentication context
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                logger.LogError("No external login info found.");
                return Results.BadRequest("No external login info found.");
            }
            
            // Extract user information from external provider claims
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var provider = info.LoginProvider;
            
            if (!string.IsNullOrEmpty(action) && action == "LoginCallback")
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
           [FromQuery] string? action,
           [FromQuery] string? tenantId,
           [FromQuery] string? timezoneId,
           [FromQuery] string? languageCode) =>
        {
            // Retrieve external login information for account linking
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                logger.LogWarning("No external login info found for linking.");
                return Results.BadRequest("No external login info found for linking.");
            }
            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(timezoneId) || string.IsNullOrEmpty(languageCode))
            {
                return Results.BadRequest("Tenant ID, timezone ID, and language code are required for linking external login.");
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
                    logger.LogError("Failed to create role.");
                    return Results.BadRequest("Failed to create role for tenant.");
                }
            }
            
            // Create the user account
            var userResult = await userManager.CreateAsync(user);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to create user.");
                return Results.BadRequest("Failed to create user.");
            }
            
            // Assign the user to the basic role
            userResult = await userManager.AddToRoleAsync(user, role.Name!);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to add user to role for user {UserId}", user.Id);
                return Results.BadRequest("Failed to add user to role.");
            }
            
            // Link the external login to the user account
            userResult = await userManager.AddLoginAsync(user, info);
            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to add external login for user {UserId} with provider {Provider}", user.Id, info.LoginProvider);
                return Results.BadRequest("Failed to add external login.");
            }
            
            if (action == "LoginCallback")
            {
                // Sign in the newly created user
                await signInManager.SignInAsync(user, true);
                logger.LogInformation("User {UserId} linked external account {Provider} successfully.", user.Id, info.LoginProvider);
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
