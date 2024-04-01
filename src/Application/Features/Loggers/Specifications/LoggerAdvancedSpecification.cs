namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
    // Obtain the system's current time zone
    var localZone = TimeZoneInfo.Local;
    // Get the current UTC time
    var utcNow = DateTime.UtcNow;
    // Convert the current UTC time to the system's local time
    var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localZone);
    // Determine today's date based on the system's local time
    var localToday = localNow.Date;
    // Convert the start and end of the local day back to UTC time for database querying
    var startOfLocalTodayAsUtc = TimeZoneInfo.ConvertTimeToUtc(localToday, localZone);
    var endOfLocalTodayAsUtc = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1).AddTicks(-1), localZone);
    // Calculate the start of the last 30 days in local time, then convert to UTC for the query
    var startOfLast30DaysLocalAsUtc = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(-30), localZone);

    // Construct the query conditions
    Query.Where(p => p.TimeStamp >= startOfLocalTodayAsUtc && p.TimeStamp < endOfLocalTodayAsUtc, filter.ListView == LogListView.CreatedToday)
         .Where(p => p.TimeStamp >= startOfLast30DaysLocalAsUtc, filter.ListView == LogListView.Last30days)
         .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
         .Where(x => x.Message.Contains(filter.Keyword) || x.Exception.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
}
