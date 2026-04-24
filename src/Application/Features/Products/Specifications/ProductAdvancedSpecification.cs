namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;
#nullable disable warnings
public class ProductAdvancedSpecification : Specification<Product>
{
    public ProductAdvancedSpecification(ProductAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange("TODAY",filter.CurrentUser.LocalTimeOffset);
        var last30daysrange = today.GetDateRange("LAST_30_DAYS", filter.CurrentUser.LocalTimeOffset);
        Query.Where(x => x.Name != null)
            .Where(x => x.Name.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword) ||
                        x.Brand.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
            .Where(x => x.Name.Contains(filter.Name), !string.IsNullOrEmpty(filter.Name))
            .Where(x => x.Unit == filter.Unit, !string.IsNullOrEmpty(filter.Unit))
            .Where(x => x.Brand == filter.Brand, !string.IsNullOrEmpty(filter.Brand))
            .Where(x => x.Price <= filter.MaxPrice, filter.MaxPrice is not null)
            .Where(x => x.Price >= filter.MinPrice, filter.MinPrice is not null)
            .Where(x => x.CreatedById == filter.CurrentUser.UserId, filter.ListView == ProductListView.My)
            .Where(x => x.CreatedAt >= todayrange.Start && x.CreatedAt < todayrange.End.AddDays(1), filter.ListView == ProductListView.TODAY)
            .Where(x => x.CreatedAt >= last30daysrange.Start, filter.ListView == ProductListView.LAST_30_DAYS);
    }
}
