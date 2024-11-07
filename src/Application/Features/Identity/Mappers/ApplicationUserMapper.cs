using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Mappers;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
[UseStaticMapper(typeof(TenantMapper))]
public static partial class ApplicationUserMapper
{
    [MapPropertyFromSource(nameof(ApplicationUser.CreatedByUser), Use = nameof(MapWithoutRelatedProperties))]
    [MapPropertyFromSource(nameof(ApplicationUser.LastModifiedByUser), Use = nameof(MapWithoutRelatedProperties))]
    [MapPropertyFromSource(nameof(ApplicationUser.Superior), Use = nameof(MapWithoutRelatedProperties))]
    [MapProperty(nameof(ApplicationUser.UserRoles), nameof(ApplicationUserDto.AssignedRoles), Use = nameof(MapAssignedRoles))]
    public static partial ApplicationUserDto ToApplicationUserDto(ApplicationUser user);

    [MapperIgnoreSource(nameof(ApplicationUser.CreatedByUser))]
    [MapperIgnoreSource(nameof(ApplicationUser.LastModifiedByUser))]
    [MapperIgnoreSource(nameof(ApplicationUser.Superior))]
    private static partial ApplicationUserDto MapWithoutRelatedProperties(ApplicationUser user);

    public static partial IQueryable<ApplicationUserDto> ProjectTo(this IQueryable<ApplicationUser> q);

    private static string[] MapAssignedRoles(ICollection<ApplicationUserRole> roles)
    {
        return roles.Select(r => r.Role.Name!).ToArray();
    }
}
