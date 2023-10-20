using CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Customers.Components;
public partial class CustomersAdvancedSearchComponent
{
    [EditorRequired][Parameter] public CustomersWithPaginationQuery TRequest { get; set; } = null!;
    [EditorRequired][Parameter] public EventCallback<string> OnConditionChanged { get; set; }
    private bool _advancedSearchExpanded;
    private async Task TextChanged(string str)
    {
        if (_advancedSearchExpanded)
        {
            await OnConditionChanged.InvokeAsync(str);
        }
    }
}