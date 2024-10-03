using CleanArchitecture.Blazor.Application.Features.Loggers.Queries.PaginationQuery;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
    public LoggerAdvancedSpecification(LogsWithPaginationQuery filter)
    {
        var timezoneOffset = filter.LocalTimezoneOffset;
        var utcNow = DateTime.UtcNow;
        // Corrected: Add the time zone offset to UTC time to get local time
        var localNow = utcNow.AddHours(timezoneOffset);

        // Calculate the start and end of today in local time
        var startOfTodayLocal = localNow.Date;
        var endOfTodayLocal = startOfTodayLocal.AddDays(1);
        var startOfLast30DaysLocal = startOfTodayLocal.AddDays(-30);

        // Convert local times back to UTC to match the TimeStamp's time zone
        var startOfTodayLocalAsUtc = startOfTodayLocal.AddHours(-timezoneOffset);
        var endOfTodayLocalAsUtc = endOfTodayLocal.AddHours(-timezoneOffset);
        var startOfLast30DaysLocalAsUtc = startOfLast30DaysLocal.AddHours(-timezoneOffset);

        // Build query conditions
        Query.Where(p => p.TimeStamp >= startOfTodayLocalAsUtc && p.TimeStamp < endOfTodayLocalAsUtc,
                filter.ListView == LogListView.CreatedToday)
            .Where(p => p.TimeStamp >= startOfLast30DaysLocalAsUtc, filter.ListView == LogListView.Last30days)
            .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
            .Where(x =>x.Message.Contains(filter.Keyword),!string.IsNullOrEmpty(filter.Keyword));
    }
}