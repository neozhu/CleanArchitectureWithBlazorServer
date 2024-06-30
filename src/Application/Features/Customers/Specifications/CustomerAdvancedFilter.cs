namespace CleanArchitecture.Blazor.Application.Features.Customers.Specifications;

#nullable disable warnings
/// <summary>
/// Specifies the different views available for the Customer list.
/// </summary>
public enum CustomerListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Toady")]
    CreatedToday,
    [Description("Created within the last 30 days")]
    Created30Days
}
/// <summary>
/// A class for applying advanced filtering options to Customer lists.
/// </summary>
public class CustomerAdvancedFilter: PaginationFilter
{
    public CustomerListView ListView { get; set; } = CustomerListView.All;
    public UserProfile? CurrentUser { get; set; }
}