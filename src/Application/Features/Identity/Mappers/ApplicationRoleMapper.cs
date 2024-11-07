using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class ApplicationRoleMapper
{
    [MapProperty("Tenant.Name", "TenantName")]
    public static partial ApplicationRoleDto ToApplicationRoleDto(ApplicationRole role);
    public static partial IQueryable<ApplicationRoleDto> ProjectTo(this IQueryable<ApplicationRole> q);

}
