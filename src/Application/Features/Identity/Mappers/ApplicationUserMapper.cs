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
    [MapperIgnoreSource(nameof(ApplicationUser.Tenant))]
    private static partial ApplicationUserDto MapWithoutRelatedProperties(ApplicationUser user);
    public static IQueryable<ApplicationUserDto> ProjectTo(this IQueryable<ApplicationUser> q)
    {
        return q.Select(x => new ApplicationUserDto
        {
            Id = x.Id,
            UserName = x.UserName ?? "",
            DisplayName = x.DisplayName,
            Provider = x.Provider,
            TenantId = x.TenantId,
            Tenant = x.Tenant != null ? TenantMapper.ToDto(x.Tenant) : default,
            ProfilePictureDataUrl = x.ProfilePictureDataUrl,
            Email = x.Email ?? "",
            PhoneNumber = x.PhoneNumber,
            Superior = x.Superior != null ? MapWithoutRelatedProperties(x.Superior) : null,
            CreatedByUser = x.CreatedByUser != null ? MapWithoutRelatedProperties(x.CreatedByUser) : null,
            LastModifiedByUser = x.LastModifiedByUser != null ? MapWithoutRelatedProperties(x.LastModifiedByUser) : null,
            AssignedRoles = x.UserRoles.Select(r => r.Role.Name!).ToArray(),
            IsActive = x.IsActive,
            IsLive = x.IsLive,
            EmailConfirmed = x.EmailConfirmed,
            LockoutEnd = x.LockoutEnd,
            TimeZoneId = x.TimeZoneId,
            LanguageCode = x.LanguageCode,
            LastModified = x.LastModified,
            LastModifiedBy = x.LastModifiedBy,
            Created = x.Created,
            CreatedBy = x.CreatedBy
        });
    }


    private static string[] MapAssignedRoles(ICollection<ApplicationUserRole> roles)
    {
        return roles.Select(r => r.Role.Name!).ToArray();
    }
}
