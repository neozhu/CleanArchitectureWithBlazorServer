using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Mappers;
using CleanArchitecture.Blazor.Domain.Identity;
using Riok.Mapperly.Abstractions;

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
    public static partial ApplicationUserDto ToApplicationUserDto(ApplicationUser user);

    // Custom mapping method to map only direct properties, excluding navigation properties
    public static ApplicationUserDto MapWithoutRelatedProperties(ApplicationUser user)
    {
        if (user == null)
            return null;

        return new ApplicationUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Provider = user.Provider,
            TenantId = user.TenantId,
            ProfilePictureDataUrl = user.ProfilePictureDataUrl,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            SuperiorId = user.SuperiorId,
            IsActive = user.IsActive,
            IsLive = user.IsLive,
            EmailConfirmed = user.EmailConfirmed,
            LockoutEnd = user.LockoutEnd,
            TimeZoneId = user.TimeZoneId,
            LanguageCode = user.LanguageCode,
            LastModified = user.LastModified,
            LastModifiedBy = user.LastModifiedBy,
            Created = user.Created,
            CreatedBy = user.CreatedBy
            // Ignoring navigation properties like Superior, CreatedByUser, LastModifiedByUser
        };
    }

     
}
