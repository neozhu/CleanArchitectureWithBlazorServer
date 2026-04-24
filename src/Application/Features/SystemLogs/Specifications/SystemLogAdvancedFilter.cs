using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Specifications;

public enum SystemLogListView
{
    [Display(Name = "All", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))] 
    All,
    [Display(Name = "Created_today", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))] 
    TODAY,
    [Display(Name = "Created_in_the_last_30_days", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))]
    LAST_30_DAYS
}

public class SystemLogAdvancedFilter : PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }
    public LogLevel? Level { get; set; }
    public SystemLogListView ListView { get; set; } = SystemLogListView.LAST_30_DAYS;
}
