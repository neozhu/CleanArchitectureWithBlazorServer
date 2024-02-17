using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;

public class ProductAdvancedFilter : PaginationFilter
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }

    public ProductListView ListView { get; set; } =
        ProductListView.All; //<-- When the user selects a different ListView,

    public UserProfile?
        CurrentUser { get; set; } // <-- This CurrentUser property gets its value from the information of
}