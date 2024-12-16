

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Specifications;

#nullable disable warnings
/// <summary>
/// Specifies the different views available for the SupplyItem list.
/// </summary>
public enum SupplyItemListView
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
/// <summary>
/// A class for applying advanced filtering options to SupplyItem lists.
/// </summary>
public class SupplyItemAdvancedFilter: PaginationFilter
{
    public TimeSpan LocalTimezoneOffset { get; set; }
    public SupplyItemListView ListView { get; set; } = SupplyItemListView.All;
    public UserProfile? CurrentUser { get; set; }
}