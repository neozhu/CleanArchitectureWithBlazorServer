using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using Mapster;

namespace CleanArchitecture.Blazor.Application.Common.Mappings;

public static class MapsterConfiguration
{
    public static TypeAdapterConfig Create()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<Document, DocumentDto>()
            .Map(dest => dest.TenantName, src => src.Tenant == null ? null : src.Tenant.Name);

        config.NewConfig<ApplicationUser, ApplicationUserDto>()
            .Ignore(dest => dest.LocalTimeOffset)
            .Map(dest => dest.AssignedRoles, src => src.UserRoles.Select(role => role.Role.Name).ToArray())
            .Map(dest => dest.Tenants, src => src.TenantUsers.Select(tenantUser => tenantUser.Tenant))
            .Map(dest => dest.Superior, src => src.Superior == null
                ? null
                : new ApplicationUserDto
                {
                    Id = src.Superior.Id,
                    UserName = src.Superior.UserName,
                    DisplayName = src.Superior.DisplayName,
                    Email = src.Superior.Email,
                    PhoneNumber = src.Superior.PhoneNumber,
                    ProfilePictureDataUrl = src.Superior.ProfilePictureDataUrl,
                    IsActive = src.Superior.IsActive,
                    TenantId = src.Superior.TenantId,
                    TimeZoneId = src.Superior.TimeZoneId,
                    LanguageCode = src.Superior.LanguageCode
                });

        return config;
    }
}
