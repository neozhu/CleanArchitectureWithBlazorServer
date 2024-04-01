namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
    public LoggerAdvancedSpecification(LoggerAdvancedFilter filter)
    {
        // Convert to UTC to ensure consistency in time comparisons
        var utcNow = DateTime.UtcNow;
        var today = utcNow.Date; // Gets today's date with time set to 00:00:00
        var startOfToday = today; // Start of today, already at 00:00:00
        var endOfToday = today.AddHours(23).AddMinutes(59).AddSeconds(59); // End of today at 23:59:59
        var startOfLast30Days = today.AddDays(-30); // Start date for the last 30 days filter

        // Build the query conditions
        Query.Where(p => p.TimeStamp >= startOfToday && p.TimeStamp <= endOfToday, filter.ListView == LogListView.CreatedToday)
             .Where(p => p.TimeStamp >= startOfLast30Days, filter.ListView == LogListView.Last30days)
             .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
             .Where(x => x.Message.Contains(filter.Keyword) || x.Exception.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}