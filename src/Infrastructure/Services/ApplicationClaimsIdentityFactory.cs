// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using CleanArchitecture.Blazor.Application.Constants.ClaimTypes;
using Newtonsoft.Json;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ApplicationClaimsIdentityFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly CustomUserManager _userManager;

    public ApplicationClaimsIdentityFactory(CustomUserManager userManager,
        CustomRoleManager roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
    }
    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        if (user.UserRoles != null && user.UserRoles.Any())
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.UserRoles,JsonConvert.SerializeObject(user.UserRoles))
            });
        }
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
        if (!string.IsNullOrEmpty(user.SuperiorId))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.SuperiorId, user.SuperiorId)
            });
        }
        if (!string.IsNullOrEmpty(user.DisplayName))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            });
        }
        if (!string.IsNullOrEmpty(user.ProfilePictureDataUrl))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl)
            });
        }
        var appuser = await _userManager.FindByIdAsync(user.Id);
        var roles = await _userManager.GetRolesAsync(appuser);
        if (roles != null && roles.Count > 0)
        {
            var rolesStr = string.Join(",", roles);
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr)
            });
        }
        return principal;
    }
}
