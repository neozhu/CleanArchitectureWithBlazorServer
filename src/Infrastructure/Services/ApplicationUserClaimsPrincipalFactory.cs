// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using System.Text.Json;
using Newtonsoft.Json;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Domain.Enums;
using Common;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    //private readonly CustomUserManager _userManager;
    ITenantService _tenantsService;
    public ApplicationUserClaimsPrincipalFactory(CustomUserManager userManager,
        CustomRoleManager roleManager,
        IOptions<IdentityOptions> optionsAccessor, ITenantService tenantsService) : base(userManager, roleManager, optionsAccessor)
    {
        _tenantsService = tenantsService;
        //_userManager = userManager;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        
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
            //since UserRoleTenants are coming in sorted order from db itself so no more sorting again
            //var roleseListDescendingOrder = EnumExtensions.SortByEnum<string, RoleNamesEnum>(user.UserRoleTenants.Select(x => x.RoleName).ToList(), descending: true);
            var rolesStr = string.Join(",", user.UserRoleTenants.Select(x => x.RoleName));
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr)
            });//sort these roles in order

            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.UserRoleTenants,JsonConvert.SerializeObject(user.UserRoleTenants))
            });

            //TODO if user has changed his default role then logic wont work,instead everytime switches to top role on login
            //TODO need to think what is best
            var allowedTenants = _tenantsService.GetAllowedTenants(user);

            ((ClaimsIdentity)principal.Identity)?.AddClaims(
            [
                new Claim(ApplicationClaimTypes.TenantId, allowedTenants.FirstOrDefault().Id)
            ]);

            ((ClaimsIdentity)principal.Identity)?.AddClaims(
            [
                new Claim(ApplicationClaimTypes.TenantName, allowedTenants.FirstOrDefault().Name)
            ]);
        }
        else
        {
            if (!string.IsNullOrEmpty(user.TenantId))
            {
                ((ClaimsIdentity)principal.Identity)?.AddClaims(
                [
                    new Claim(ApplicationClaimTypes.TenantId, user.TenantId)
                ]);
            }

            if (!string.IsNullOrEmpty(user.TenantName))
            {
                ((ClaimsIdentity)principal.Identity)?.AddClaims(
                [
                    new Claim(ApplicationClaimTypes.TenantName, user.TenantName)
                ]);
            }
        }





        if (!string.IsNullOrEmpty(user.SuperiorId))//todo this had to be based on tenant specific ,its pending as of now
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim(ApplicationClaimTypes.SuperiorId, user.SuperiorId)
            });
        return principal;
    }
}