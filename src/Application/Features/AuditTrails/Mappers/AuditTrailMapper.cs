using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Mappers;
using Riok.Mapperly.Abstractions;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Mappers;
[Mapper]
[UseStaticMapper(typeof(ApplicationUserMapper))]
public static partial class AuditTrailMapper
{
    [MapPropertyFromSource(nameof(AuditTrail.PrimaryKey), Use = nameof(MapPrimaryKey))]
    public static partial IQueryable<AuditTrailDto> ProjectTo(this IQueryable<AuditTrail> q);
    [MapPropertyFromSource(nameof(AuditTrail.PrimaryKey), Use = nameof(MapPrimaryKey))]
    public static partial AuditTrailDto ToDto(AuditTrail auditTrail);

    private static string MapPrimaryKey(AuditTrail source)
    {
        return JsonSerializer.Serialize(source.PrimaryKey, new JsonSerializerOptions());
    }
}
