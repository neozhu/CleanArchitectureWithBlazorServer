// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;
using CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Loggers.Mappers;
using CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Queries.PaginationQuery;

public class LogsWithPaginationQuery : LoggerAdvancedFilter, ICacheableRequest<PaginatedData<LogDto>>
{
    public LoggerAdvancedSpecification Specification => new(this);

    public string CacheKey => LogsCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => LogsCacheKey.Tags;

    public override string ToString()
    {
        return
            $"Listview:{ListView}-{LocalTimeOffset.TotalHours},{Level},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class LogsQueryHandler : IRequestHandler<LogsWithPaginationQuery, PaginatedData<LogDto>>
{
    private readonly IApplicationDbContext _context;

    public LogsQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<PaginatedData<LogDto>> Handle(LogsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.Loggers.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync(request.Specification, request.PageNumber, request.PageSize,
                LogMapper.ToDto, cancellationToken);
        return data;
    }
}