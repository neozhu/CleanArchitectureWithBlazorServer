// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Domain.Events;

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
        return result;
    }

    public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
    {
        var result = await base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);

        // Try to get user information from external login
        var info = await GetExternalLoginInfoAsync();
        var email= info?.Principal.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
        var user = await UserManager.FindByEmailAsync(email);
        if (user != null) {
            var userId = await UserManager.GetUserIdAsync(user);
            var userName = await UserManager.GetUserNameAsync(user);
        }
        return result;
    }

    public override async Task SignInAsync(TUser user, bool isPersistent, string? authenticationMethod = null)
    {
        await base.SignInAsync(user, isPersistent, authenticationMethod);
        var userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
        var userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
    }

    public override async Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string? authenticationMethod = null)
    {
        await base.SignInAsync(user, authenticationProperties, authenticationMethod);
        var userName = await UserManager.GetUserNameAsync(user) ?? "Unknown";
        var userId = await UserManager.GetUserIdAsync(user) ?? "Unknown";
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

        return result;
    }

    

     
}
