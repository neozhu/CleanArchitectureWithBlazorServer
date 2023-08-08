// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Queries.PaginationQuery;
using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;
using CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;
using CleanArchitecture.Blazor.Domain.Entities.Logger;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Queries.PaginationQuery;

public class LogsWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<LogDto>>
{
    public LogLevel? Level { get; set; }
    public LogListView ListView { get; set; } = LogListView.All;
    public string CacheKey => LogsCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => LogsCacheKey.MemoryCacheEntryOptions;
    public override string ToString()
    {
        return $"Listview:{ListView},{Level},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
    public LogsQuerySpec Specification => new LogsQuerySpec(this);
}

public class LogsQueryHandler : IRequestHandler<LogsWithPaginationQuery, PaginatedData<LogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LogsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedData<LogDto>> Handle(LogsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.Loggers.Specify(request.Specification)
            .ProjectTo<LogDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.PageNumber, request.PageSize);

        return data;
    }
}

public class LogsQuerySpec : Specification<Logger>
{
    public LogsQuerySpec(LogsWithPaginationQuery request)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30days =
            Convert.ToDateTime(today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
                CultureInfo.CurrentCulture);
        Query.Where(p => p.TimeStamp.Date == DateTime.Now.Date, request.ListView == LogListView.CreatedToday)
             .Where(p => p.TimeStamp >= last30days, request.ListView == LogListView.Last30days)
             .Where(p => p.Level== request.Level.ToString(), request.Level is not null)
             .Where(x => x.Message.Contains(request.Keyword) || x.Exception.Contains(request.Keyword) || x.UserName.Contains(request.Keyword), !string.IsNullOrEmpty(request.Keyword));
    }
}

public enum LogListView
{
    [Description("All")] All,
    [Description("Created Toady")] CreatedToday,
    [Description("View of the last 30 days")]
    Last30days
}