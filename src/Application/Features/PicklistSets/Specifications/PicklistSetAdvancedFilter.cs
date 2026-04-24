using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;

public enum PickListView
{
    [Display(Name = "All", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))]
    All,
    [Display(Name =  "Created_by_me", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))]
    My,
    [Display(Name = "Created_today", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))]
    TODAY,
    [Display(Name = "Created_in_the_last_30_days", ResourceType = typeof(CleanArchitecture.Blazor.Application.Resources.Constants.AppStrings))]
    LAST_30_DAYS
}
public class PicklistSetAdvancedFilter : PaginationFilter
{
    public PickListView ListView { get; set; } = PickListView.All;
    public UserProfile? CurrentUser { get; set; }
    public Picklist? Picklist { get; set; }
}
