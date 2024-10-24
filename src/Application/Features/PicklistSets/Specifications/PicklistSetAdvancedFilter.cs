namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;

public enum PickListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Toady")]
    TODAY,
    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}
public class PicklistSetAdvancedFilter : PaginationFilter
{
    public PickListView ListView { get; set; } = PickListView.All;
    public UserProfile? CurrentUser { get; set; }
    public Picklist? Picklist { get; set; }
}