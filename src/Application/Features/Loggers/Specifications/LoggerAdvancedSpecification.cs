namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
    public LoggerAdvancedSpecification(LoggerAdvancedFilter filter)
    {
        var today = DateTime.Now.ToUniversalTime().Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30days =
            Convert.ToDateTime(today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
                CultureInfo.CurrentCulture);
        Query.Where(p => p.TimeStamp.Date == DateTime.Now.Date, filter.ListView == LogListView.CreatedToday)
            .Where(p => p.TimeStamp >= last30days, filter.ListView == LogListView.Last30days)
            .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
            .Where(
                x => x.Message.Contains(filter.Keyword) || x.Exception.Contains(filter.Keyword)  , !string.IsNullOrEmpty(filter.Keyword));
    }
}