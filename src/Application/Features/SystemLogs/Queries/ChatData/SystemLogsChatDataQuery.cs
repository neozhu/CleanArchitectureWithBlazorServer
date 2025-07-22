// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.SystemLogs.Caching;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Queries.ChatData;

public class SystemLogsTimeLineChatDataQuery : ICacheableRequest<List<SystemLogTimeLineDto>>
{
    public DateTime LastDateTime { get; set; } = DateTime.Now.AddDays(-60);
    public string CacheKey => SystemLogsCacheKey.GetChartDataCacheKey(LastDateTime.ToString());
    public IEnumerable<string>? Tags => SystemLogsCacheKey.Tags;
}

public class SystemLogsChatDataQueryHandler : IRequestHandler<SystemLogsTimeLineChatDataQuery, List<SystemLogTimeLineDto>>

{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public SystemLogsChatDataQueryHandler(
        IApplicationDbContextFactory dbContextFactory,
        IMapper mapper
    )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<List<SystemLogTimeLineDto>> Handle(SystemLogsTimeLineChatDataQuery request,
        CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.SystemLogs.Where(x => x.TimeStamp >= request.LastDateTime)
            .GroupBy(x => new { x.TimeStamp.Date })
            .Select(x => new { x.Key.Date, Total = x.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        List<SystemLogTimeLineDto> result = new();
        DateTime end = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        var start = request.LastDateTime.Date;

        while (start <= end)
        {
            var item = data.FirstOrDefault(x => x.Date == start.Date);
            result.Add(item != null
                ? new SystemLogTimeLineDto { dt = item.Date, total = item.Total }
                : new SystemLogTimeLineDto { dt = start, total = 0 });

            start = start.AddDays(1);
        }

        return result.OrderBy(x => x.dt).ToList();
    }
}