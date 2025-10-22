// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net; // added for IPAddress parsing

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
        var userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
        await LogLoginAuditAsync(userId, userName, result.Succeeded, "Local");
        return result;
    }

    public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
    {
        var result = await base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);

        // Try to get user information from external login
        var info = await GetExternalLoginInfoAsync();
        var userName = info?.Principal?.Identity?.Name ?? "External User";
        var userId = info?.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
        await LogLoginAuditAsync(userId, userName, result.Succeeded, loginProvider);
        return result;
    }

    public override async Task SignInAsync(TUser user, bool isPersistent, string? authenticationMethod = null)
    {
        await base.SignInAsync(user, isPersistent, authenticationMethod);
        var userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
        var userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
        await LogLoginAuditAsync(userId, userName, true, authenticationMethod ?? "Direct");
    }

    public override async Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string? authenticationMethod = null)
    {
        await base.SignInAsync(user, authenticationProperties, authenticationMethod);
        var userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
        var userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
        await LogLoginAuditAsync(userId, userName, true, authenticationMethod ?? "Direct");
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

        await LogLoginAuditAsync(userId, userName, result.Succeeded, $"2FA-{provider}");
        return result;
    }

    private async Task LogLoginAuditAsync(string userId, string userName, bool success, string provider)
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
            var loginAudit = new LoginAudit()
            {
                LoginTimeUtc = DateTime.UtcNow,
                UserId = userId ?? string.Empty,
                UserName = userName,
                IpAddress = ipAddress,
                BrowserInfo = browserInfo,
                Provider = provider,
                Success = success
            };
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
            // Simple & safe: only examine a short list of common headers, validate each with IPAddress.TryParse.
            // Order: CF-Connecting-IP -> X-Forwarded-For (first) -> True-Client-IP -> X-Real-IP -> fallback RemoteIpAddress
            if (TryGetSingleHeaderIp(httpContext, "CF-Connecting-IP", out var ip)) return ip;
            if (TryGetXForwardedFor(httpContext, out ip)) return ip;
            if (TryGetSingleHeaderIp(httpContext, "True-Client-IP", out ip)) return ip;
            if (TryGetSingleHeaderIp(httpContext, "X-Real-IP", out ip)) return ip;

            var remote = httpContext.Connection.RemoteIpAddress;
            return NormalizeLoopback(remote);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get client IP address");
            return null;
        }
    }

    private bool TryGetSingleHeaderIp(HttpContext ctx, string headerName, out string? ip)
    {
        ip = null;
        var raw = ctx.Request.Headers[headerName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(raw)) return false;
        raw = raw.Split(',')[0].Trim(); // if multiple, only first
        raw = StripPortAndBrackets(raw);
        if (IPAddress.TryParse(raw, out var parsed))
        {
            ip = NormalizeLoopback(parsed);
            return true;
        }
        return false;
    }

    private bool TryGetXForwardedFor(HttpContext ctx, out string? ip)
    {
        ip = null;
        var raw = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(raw)) return false;
        // Split chain client, proxy1, proxy2 ... choose first non-empty candidate that parses
        foreach (var candidate in raw.Split(',').Select(s => s.Trim()))
        {
            if (string.IsNullOrEmpty(candidate)) continue;
            var cleaned = StripPortAndBrackets(candidate);
            if (IPAddress.TryParse(cleaned, out var parsed))
            {
                ip = NormalizeLoopback(parsed);
                return true;
            }
        }
        return false;
    }

    private string StripPortAndBrackets(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        value = value.Trim('"');
        // IPv6 in brackets: [2001:db8::1]:443
        if (value.StartsWith("[") && value.Contains("]"))
        {
            var end = value.IndexOf(']');
            if (end > 0)
            {
                var core = value.Substring(1, end - 1);
                // ignore trailing :port
                return core;
            }
        }
        // IPv4:port
        var colonIndex = value.LastIndexOf(':');
        if (colonIndex > -1 && value.Count(c => c == ':') == 1 && value.Contains('.'))
        {
            return value.Substring(0, colonIndex);
        }
        return value;
    }

    private string? NormalizeLoopback(IPAddress? ip)
    {
        if (ip == null) return null;
        if (IPAddress.IsLoopback(ip)) return "127.0.0.1"; // unify
        return ip.ToString();
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
