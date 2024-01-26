using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Infrastructure;
public class StaticData//only for first time
{
    public static List<ApplicationRoleDto>? Roles = null;
    public static List<TenantDto>? Tenants = null;
    readonly IIdentityService _identityService;
    readonly ITenantService _tenantService;
    public StaticData(IIdentityService identityService, ITenantService tenantService)
    {
        _identityService = identityService;
        _tenantService = tenantService;
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

    public async Task<List<TenantDto>> LoadAllTenants(bool forceLoad = false)
    {
        if (forceLoad || Tenants == null || !Tenants.Any())
            Tenants = _tenantService.DataSource;
        return Tenants;
    }
}
