// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using System.Text.Json;
using Newtonsoft.Json;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly CustomUserManager _userManager;

    public ApplicationUserClaimsPrincipalFactory(CustomUserManager userManager,
        CustomRoleManager roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        if (user.UserRoleTenants != null && user.UserRoleTenants.Any())
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.UserRoles,JsonConvert.SerializeObject(user.UserRoleTenants))
            });
        }
        if (!string.IsNullOrEmpty(user.TenantId))
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.TenantId, user.TenantId)
            });
        if (!string.IsNullOrEmpty(user.TenantName))
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.TenantName, user.TenantName)
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
        //  var appUser = await UserManager.FindByIdAsync(user.Id);//this can be redundant
        if (user.UserRoleTenants != null && user.UserRoleTenants.Any())
        {
            var rolesStr = string.Join(",", user.UserRoleTenants.Select(x => x.RoleName));
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr)
            });
        }

        return principal;
    }
}