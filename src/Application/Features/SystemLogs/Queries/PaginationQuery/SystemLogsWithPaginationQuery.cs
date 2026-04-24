// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Mapster;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.Caching;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Queries.PaginationQuery;

public class SystemLogsWithPaginationQuery : SystemLogAdvancedFilter, ICacheableRequest<PaginatedData<SystemLogDto>>
{
    public SystemLogAdvancedSpecification Specification => new(this);

    public string CacheKey => SystemLogsCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => SystemLogsCacheKey.Tags;

    public override string ToString()
    {
        return
            $"Listview:{ListView},{Level},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class LogsQueryHandler : IRequestHandler<SystemLogsWithPaginationQuery, PaginatedData<SystemLogDto>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly TypeAdapterConfig _typeAdapterConfig;

    public LogsQueryHandler(
        IApplicationDbContextFactory dbContextFactory,
        TypeAdapterConfig typeAdapterConfig
    )
    {
        _dbContextFactory = dbContextFactory;
        _typeAdapterConfig = typeAdapterConfig;
    }

    public async ValueTask<PaginatedData<SystemLogDto>> Handle(SystemLogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.SystemLogs.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<SystemLog, SystemLogDto>(request.Specification, request.PageNumber, request.PageSize, _typeAdapterConfig, cancellationToken);
        return data;
    }
}
