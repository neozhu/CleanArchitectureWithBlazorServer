using CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Products;
public partial class AdvancedSearchProductsComponent
{
    [EditorRequired][Parameter] public ProductsWithPaginationQuery TRequest { get; set; } = default!;
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