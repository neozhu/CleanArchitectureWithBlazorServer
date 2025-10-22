// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class AuditSignInManager<TUser> : SignInManager<TUser>
    where TUser : class
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditSignInManager<TUser>> _logger;

    public AuditSignInManager(
        UserManager<TUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<TUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<TUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<TUser> confirmation,
        IApplicationDbContextFactory dbContextFactory,
        ILogger<AuditSignInManager<TUser>> auditLogger)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _dbContextFactory = dbContextFactory;
        _httpContextAccessor = contextAccessor;
        _logger = auditLogger;
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        // Just call base method without logging here to avoid duplicate records
        // The logging will be handled by the TUser overload which gets called internally
        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

    public override async Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var result = await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        var userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
        var userId= await UserManager.GetUserIdAsync(user) ?? "Unknown";
        await LogLoginAuditAsync(userId,userName, result.Succeeded, "Local", result);
        return result;
    }

    public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
    {
        var result = await base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
        
        // Try to get user information from external login
        var info = await GetExternalLoginInfoAsync();
        var userName = info?.Principal?.Identity?.Name ?? "External User";
        var userId = info?.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
        await LogLoginAuditAsync(userId, userName, result.Succeeded, loginProvider, result);
        return result;
    }

    public override async Task SignInAsync(TUser user, bool isPersistent, string? authenticationMethod = null)
    {
        await base.SignInAsync(user, isPersistent, authenticationMethod);
        var userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
        var userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
        await LogLoginAuditAsync(userId,userName, true, authenticationMethod ?? "Direct", null);
    }

    public override async Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string? authenticationMethod = null)
    {
        await base.SignInAsync(user, authenticationProperties, authenticationMethod);
        var userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
        var userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
        await LogLoginAuditAsync(userId, userName, true, authenticationMethod ?? "Direct", null);
    }

    public override async Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
    {
        var result = await base.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);
        
        // Get user from two factor info
        var userName = "Unknown";
        var userId = "Unknown";
        var user = await GetTwoFactorAuthenticationUserAsync();
        if (user != null)
        {
            userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
            userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
        }
        
        await LogLoginAuditAsync(userId,userName, result.Succeeded, $"2FA-{provider}", result);
        return result;
    }

    private async Task LogLoginAuditAsync(string userId,string userName, bool success, string provider, SignInResult? result)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogWarning("HttpContext is null, cannot log login audit for user {UserName}", userName);
                return;
            }

            

            // Extract client information
            var ipAddress = GetClientIpAddress(httpContext);
            var browserInfo = GetBrowserInfo(httpContext);

            // Create login audit using the service
            var loginAudit = new LoginAudit() {
                LoginTimeUtc = DateTime.UtcNow,
                UserId = userId ?? string.Empty,
                UserName=userName,
                IpAddress= ipAddress,
                BrowserInfo= browserInfo,
                Provider= provider,
                Success= success};
            loginAudit.AddDomainEvent(new Domain.Events.LoginAuditCreatedEvent(loginAudit));
            // Save to database
            await using var db = await _dbContextFactory.CreateAsync();
            await db.LoginAudits.AddAsync(loginAudit);
            await db.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log login audit for user {UserName}", userName);
            // Don't throw - login audit failure shouldn't break the login process
        }
    }

    private string? GetClientIpAddress(HttpContext httpContext)
    {
        try
        {
            // Priority order of common proxy headers
            // 1. Cloudflare / CDN specific
            var cfConnectingIp = httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(cfConnectingIp)) return SanitizeAndNormalize(cfConnectingIp);

            // 2. Standard X-Forwarded-For (may contain comma separated chain). Take first non-empty value.
            var forwardedForRaw = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedForRaw))
            {
                var first = forwardedForRaw.Split(',').Select(s => s.Trim()).FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
                if (!string.IsNullOrWhiteSpace(first)) return SanitizeAndNormalize(first);
            }

            // 3. True-Client-IP (Akamai, some CDNs)
            var trueClientIp = httpContext.Request.Headers["True-Client-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(trueClientIp)) return SanitizeAndNormalize(trueClientIp);

            // 4. X-Real-IP (nginx) single value
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIp)) return SanitizeAndNormalize(realIp);

            // 5. Forwarded header (RFC 7239) e.g. Forwarded: for=203.0.113.195;proto=https;by=203.0.113.43
            var forwarded = httpContext.Request.Headers["Forwarded"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                // Extract for= value
                var segments = forwarded.Split(';');
                foreach (var seg in segments)
                {
                    var part = seg.Trim();
                    if (part.StartsWith("for=", StringComparison.OrdinalIgnoreCase))
                    {
                        var ipPart = part.Substring(4).Trim('"');
                        // Remove IPv6 brackets if present
                        ipPart = ipPart.Trim('[', ']');
                        if (!string.IsNullOrWhiteSpace(ipPart)) return SanitizeAndNormalize(ipPart);
                    }
                }
            }

            // 6. Fallback to connection remote IP
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
            return SanitizeAndNormalize(remoteIp);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get client IP address");
            return null;
        }
    }

    private string SanitizeInput(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        // Remove newline characters and trim whitespace
        return input.Replace("\r", "").Replace("\n", "").Trim();
    }
    private string? SanitizeAndNormalize(string? input)
    {
        var value = SanitizeInput(input);
        if (string.IsNullOrEmpty(value)) return value;
        if (value == "::1" || value == "127.0.0.1") return "127.0.0.1";
        // Remove port if accidentally included (IPv4:port or [IPv6]:port)
        if (value.Contains(':'))
        {
            // For IPv6 keep colons; only strip if it's an IPv4 with a single ':' and digits afterward or bracketed IPv6 with port
            if (value.Count(c => c == ':') == 1 && value.Contains('.'))
            {
                // IPv4 with port
                value = value.Split(':')[0];
            }
            else if (value.StartsWith("[") && value.Contains("]:"))
            {
                value = value.Substring(1, value.IndexOf(']') - 1); // Extract inside brackets
            }
        }
        return value;
    }
    

    private string? GetBrowserInfo(HttpContext httpContext)
    {
        try
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
            if (string.IsNullOrEmpty(userAgent))
                return null;

            // Truncate if too long to prevent database issues
            return userAgent.Length > 1000 ? userAgent.Substring(0, 1000) : userAgent;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get browser info");
            return null;
        }
    }
}
