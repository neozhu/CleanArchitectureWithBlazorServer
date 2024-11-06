using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
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
    [MapPropertyFromSource(nameof(ApplicationUser.CreatedByUser), Use = nameof(mapWithoutRelatedProperties))]
    [MapPropertyFromSource(nameof(ApplicationUser.LastModifiedByUser), Use = nameof(mapWithoutRelatedProperties))]
    [MapPropertyFromSource(nameof(ApplicationUser.Superior), Use = nameof(mapWithoutRelatedProperties))]
    public static partial ApplicationUserDto ToApplicationUserDto(ApplicationUser user);

    [MapperIgnoreSource(nameof(ApplicationUser.CreatedByUser))]
    [MapperIgnoreSource(nameof(ApplicationUser.LastModifiedByUser))]
    [MapperIgnoreSource(nameof(ApplicationUser.Superior))]
    private static partial ApplicationUserDto mapWithoutRelatedProperties(ApplicationUser user);
}
