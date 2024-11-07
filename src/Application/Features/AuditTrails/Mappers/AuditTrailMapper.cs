using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
[UseStaticMapper(typeof(ApplicationUserMapper))]
public static partial class AuditTrailMapper
{
    public static partial IQueryable<AuditTrailDto> ProjectTo(this IQueryable<AuditTrail> q);
    [MapPropertyFromSource(nameof(AuditTrail.PrimaryKey), Use = nameof(MapPrimaryKey))]
    public static partial AuditTrailDto ToDto(AuditTrail auditTrail);

    private static string MapPrimaryKey(AuditTrail source)
    {
        return JsonSerializer.Serialize(source.PrimaryKey, new JsonSerializerOptions());
    }
}
