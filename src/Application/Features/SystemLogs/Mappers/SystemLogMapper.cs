using CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Mappers;
[Mapper]
public static partial class SystemLogMapper
{
    public static partial SystemLogDto ToDto(SystemLog logger);
    public static partial IQueryable<SystemLogDto> ProjectTo(this IQueryable<SystemLog> q);
}
