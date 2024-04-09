using CleanArchitecture.Blazor.Application.Features.Loggers.Queries.PaginationQuery;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
    public LoggerAdvancedSpecification(LogsWithPaginationQuery filter)
    {
        var timeZoneOffset = filter.TimeZoneOffset;
        var utcNow = DateTime.UtcNow;
        var localNow = utcNow.Date.AddHours(timeZoneOffset);
        var startOfTodayLocalAsUtc = localNow;
        var endOfTodayLocalAsUtc = localNow.AddDays(1).AddTicks(-1);
        var startOfLast30DaysLocalAsUtc = localNow.AddDays(-30);
        // 构建查询条件
        Query.Where(p => p.TimeStamp >= startOfTodayLocalAsUtc && p.TimeStamp < endOfTodayLocalAsUtc,
                filter.ListView == LogListView.CreatedToday)
            .Where(p => p.TimeStamp >= startOfLast30DaysLocalAsUtc, filter.ListView == LogListView.Last30days)
            .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
            .Where(x => x.Message.Contains(filter.Keyword) || x.Exception.Contains(filter.Keyword),
                !string.IsNullOrEmpty(filter.Keyword));
    }
}