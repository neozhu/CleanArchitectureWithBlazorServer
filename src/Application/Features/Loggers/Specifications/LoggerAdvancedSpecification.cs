namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
  public LoggerAdvancedSpecification(LoggerAdvancedFilter filter)
{
    // Obtain the current UTC time, and ensure its Kind property is correctly set to Utc
    var utcNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    // Convert the current UTC time to the local time zone without needing to adjust its Kind beforehand
    var localZone = TimeZoneInfo.Local;
    // Convert from UTC to local time to determine today's date in the local time zone
    var localToday = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localZone).Date;

    // Convert the start and end of "today" in local time back to UTC for querying.
    // Here, DateTimeKind is set to Unspecified to indicate these are local times without a specific time zone context,
    // which is necessary for the conversion method to work correctly.
    var startOfTodayLocalAsUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localToday, DateTimeKind.Unspecified), localZone);
    var endOfTodayLocalAsUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localToday.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified), localZone);

    // Similarly, convert the start of the last 30 days from local time to UTC time for the query
    var startOfLast30DaysLocalAsUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localToday.AddDays(-30), DateTimeKind.Unspecified), localZone);

    // Build the query conditions, comparing timestamps against the calculated UTC times
    // to accurately query logs created "today" and within the "last 30 days" according to the local time zone.
    Query.Where(p => p.TimeStamp >= startOfTodayLocalAsUtc && p.TimeStamp < endOfTodayLocalAsUtc, filter.ListView == LogListView.CreatedToday)
         .Where(p => p.TimeStamp >= startOfLast30DaysLocalAsUtc, filter.ListView == LogListView.Last30days)
         .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
         .Where(x => x.Message.Contains(filter.Keyword) || x.Exception.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
}

}
