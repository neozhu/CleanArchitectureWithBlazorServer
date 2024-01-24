using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Infrastructure;
public class StaticData//only for first time
{
    public static List<ApplicationRoleDto>? Roles = null;
    readonly IIdentityService _identityService;
    public StaticData(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<List<ApplicationRoleDto>> LoadUserBaseRoles(bool forceLoad = false)
    {
        if (forceLoad || Roles == null || !Roles.Any())
            Roles = (await _identityService.GetAllRoles()).OrderByDescending(r=>r.Level).ToList();
        return Roles;
    }

    public static List<ApplicationRoleDto>? RolesOfTenantType(byte tenantType)
    {
        return tenantType == 0 ? (List<ApplicationRoleDto>?)null : (Roles?.Where(x => x.TenantType == tenantType).ToList());
    }
}
