namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;

public enum ProductListView
{
    [Description("All")] All,
    [Description("My Products")] My,
    [Description("Created Today")] TODAY,

    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}