// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ApplicationClaimsIdentityFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public ApplicationClaimsIdentityFactory(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        if (!string.IsNullOrEmpty(user.TenantId))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.TenantId, user.TenantId)
            });
        }
        if (!string.IsNullOrEmpty(user.TenantName))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.TenantName, user.TenantName)
            });
        }
       
        return principal;
    }
}
