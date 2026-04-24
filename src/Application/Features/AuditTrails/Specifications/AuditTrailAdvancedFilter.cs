using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;

public enum AuditTrailListView
{
    [Display(Name = "All", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))] All,
    [Display(Name = "My_change_histories", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))] My,
    [Display(Name = "Created_today", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))] TODAY,
    [Display(Name = "Created_in_the_last_30_days", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))] LAST_30_DAYS
}

public class AuditTrailAdvancedFilter : PaginationFilter
{
    public AuditType? AuditType { get; set; }
    public AuditTrailListView ListView { get; set; } = AuditTrailListView.All;
    public UserProfile? CurrentUser { get; set; }
}
