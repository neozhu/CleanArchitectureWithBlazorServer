namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;

public enum LogListView
{
    [Description("All")] 
    All,
    [Description("Created Today")] 
    TODAY,
    [Description("View of the last 30 days")]
    LAST_30_DAYS
}

public class LoggerAdvancedFilter : PaginationFilter
{
    public TimeSpan LocalTimeOffset { get; set; }
    public LogLevel? Level { get; set; }
    public LogListView ListView { get; set; } = LogListView.LAST_30_DAYS;
}