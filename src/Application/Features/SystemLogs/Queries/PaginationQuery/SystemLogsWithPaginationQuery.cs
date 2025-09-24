// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    private readonly IMapper _mapper;

    public LogsQueryHandler(
        IApplicationDbContextFactory dbContextFactory,
        IMapper mapper
    )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<PaginatedData<SystemLogDto>> Handle(SystemLogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.SystemLogs.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<SystemLog, SystemLogDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}
