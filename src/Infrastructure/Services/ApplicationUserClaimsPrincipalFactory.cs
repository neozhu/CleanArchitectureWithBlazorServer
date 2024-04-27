// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
    {
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        if (!string.IsNullOrEmpty(user.TenantId))
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.TenantId, user.TenantId)
            });
        if (user.Tenant is not null)
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.TenantName, user.Tenant.Name)
            });
        if (!string.IsNullOrEmpty(user.SuperiorId))
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.SuperiorId, user.SuperiorId)
            });
        if (!string.IsNullOrEmpty(user.DisplayName))
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            });
        if (!string.IsNullOrEmpty(user.ProfilePictureDataUrl))
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl)
            });
        var appuser = await UserManager.FindByIdAsync(user.Id);
        var roles = await UserManager.GetRolesAsync(appuser);
        if (roles != null && roles.Count > 0)
        {
            var rolesStr = string.Join(",", roles);
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr)
            });
        }

        return principal;
    }
}