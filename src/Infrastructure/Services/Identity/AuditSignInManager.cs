// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class AuditSignInManager<TUser> : SignInManager<TUser>
    where TUser : class
{
    private readonly IApplicationDbContext _context;
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
        IApplicationDbContext context,
        ILogger<AuditSignInManager<TUser>> auditLogger)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _context = context;
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
            await _context.LoginAudits.AddAsync(loginAudit);
            await _context.SaveChangesAsync(CancellationToken.None);
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
            // Check for forwarded IP first (when behind proxy/load balancer)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // Take the first IP if there are multiple
                var firstIp = forwardedFor.Split(',').FirstOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(firstIp))
                    return firstIp;
            }

            // Check for real IP header
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
                return realIp;

            // Fall back to remote IP
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();

            // Handle localhost scenarios
            if (remoteIp == "::1" || remoteIp == "127.0.0.1")
            {
                return "127.0.0.1"; // Normalize localhost
            }

            return SanitizeInput(remoteIp);
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
