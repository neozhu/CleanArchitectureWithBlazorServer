// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Logs.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Logs.Queries.ChatData;

public class LogsTimeLineChatDataQuery : IRequest<IEnumerable<LogTimeLineDto>>
{
    public DateTime LastDateTime { get; set; } = DateTime.Now.AddDays(-3);
}
public class LogsLevelChatDataQuery : IRequest<IEnumerable<LogLevelChartDto>>
{
    public DateTime LastDateTime { get; set; } = DateTime.Now.AddDays(-3);
}
public class LogsChatDataQueryHandler :
     IRequestHandler<LogsTimeLineChatDataQuery, IEnumerable<LogTimeLineDto>>,
     IRequestHandler<LogsLevelChatDataQuery, IEnumerable<LogLevelChartDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<LogsChatDataQueryHandler> _localizer;

    public LogsChatDataQueryHandler(
        IApplicationDbContext context,
        IStringLocalizer<LogsChatDataQueryHandler> localizer
        )
    {
        _context = context;
        _localizer = localizer;
    }
    public async Task<IEnumerable<LogTimeLineDto>> Handle(LogsTimeLineChatDataQuery request, CancellationToken cancellationToken)
    {
        var levels = new string[] { "Information", "Trace", "Debug", "Warning", "Error", "Fatal" };
        var data = await _context.Loggers.Where(x => x.TimeStamp >= request.LastDateTime)
                  .GroupBy(x => new { x.Level, x.TimeStamp.Date, x.TimeStamp.Hour })
                  .Select(x => new { x.Key.Level, x.Key.Date, x.Key.Hour, Total = x.Count() })
                  .OrderBy(x => x.Level).ThenBy(x => x.Date)
                  .ToListAsync(cancellationToken);
        var result = data.Select(item => new LogTimeLineDto()
        {
            time = item.Date.AddHours(item.Hour),
            level = item.Level,
            total = item.Total
        }).OrderBy(x => x.level).ThenBy(x => x.time);
        return result;
        //var result = new List<LogTimeLineDto>();
        //var end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
        //var start = end;
        //if (data.Any())
        //{
        //    start = new DateTime(data.First().Date.Year, data.First().Date.Month, data.First().Date.Day, 0, 0, 0);
        //}
        //while (start <= end)
        //{
        //    foreach (var level in levels)
        //    {
        //        var item = data.Where(x => x.Date == start.Date && x.Hour == start.Hour && x.Level == level).FirstOrDefault();
        //        if (item != null)
        //        {
        //            result.Add(new LogTimeLineDto { time = item.Date.AddHours(item.Hour), level = level, total = item.Total });
        //        }
        //        else
        //        {
        //            result.Add(new LogTimeLineDto { time = start, level = level, total = 0 });

        //        }
        //    }
        //    start = start.AddHours(1);
        //}
        //return result;
    }

    public async Task<IEnumerable<LogLevelChartDto>> Handle(LogsLevelChatDataQuery request, CancellationToken cancellationToken)
    {
        var levels = new string[] { "Information", "Trace", "Debug", "Warning", "Error", "Fatal" };
        var data = await _context.Loggers.Where(x => x.TimeStamp >= request.LastDateTime)
                  .GroupBy(x => new { x.Level })
                  .Select(x => new LogLevelChartDto() { level = x.Key.Level, total = x.Count() })
                  .OrderBy(x => x.level)
                  .ToListAsync(cancellationToken);
        return data;

    }
}
