namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;

public enum ProductListView
{
    [Description("All")] All,
    [Description("My Products")] My,
    [Description("Created Today")] CreatedToday,

    [Description("Created within the last 30 days")]
    Created30Days
}