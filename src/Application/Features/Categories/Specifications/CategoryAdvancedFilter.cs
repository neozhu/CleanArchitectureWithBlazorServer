

namespace CleanArchitecture.Blazor.Application.Features.Categories.Specifications;

#nullable disable warnings
/// <summary>
/// Specifies the different views available for the Category list.
/// </summary>
public enum CategoryListView
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
/// A class for applying advanced filtering options to Category lists.
/// </summary>
public class CategoryAdvancedFilter: PaginationFilter
{
    public TimeSpan LocalTimezoneOffset { get; set; }
    public CategoryListView ListView { get; set; } = CategoryListView.All;
    public UserProfile? CurrentUser { get; set; }
}