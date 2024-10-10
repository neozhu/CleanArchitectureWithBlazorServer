using CleanArchitecture.Blazor.Application.Features.Loggers.Queries.PaginationQuery;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
    public LoggerAdvancedSpecification(LogsWithPaginationQuery filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange("TODAY", filter.LocalTimeOffset);
        var last30daysrange = today.GetDateRange("LAST_30_DAYS", filter.LocalTimeOffset);
        // Build query conditions
        Query.Where(p => p.TimeStamp >= todayrange.Start && p.TimeStamp < todayrange.End.AddDays(1),
                filter.ListView == LogListView.TODAY)
            .Where(p => p.TimeStamp >= last30daysrange.Start, filter.ListView == LogListView.LAST_30_DAYS)
            .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
            .Where(x =>x.Message.Contains(filter.Keyword),!string.IsNullOrEmpty(filter.Keyword));
    }
}