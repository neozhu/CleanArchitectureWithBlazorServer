using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using Riok.Mapperly.Abstractions;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Mappers;
[Mapper]
public static partial class ApplicationRoleMapper
{
    [MapProperty("Tenant.Name", "TenantName")]
    public static partial ApplicationRoleDto ToApplicationRoleDto(ApplicationRole role);
   
}
