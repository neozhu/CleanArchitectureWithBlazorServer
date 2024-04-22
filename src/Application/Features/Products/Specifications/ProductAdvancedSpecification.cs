namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;
#nullable disable warnings
public class ProductAdvancedSpecification : Specification<Product>
{
    public ProductAdvancedSpecification(ProductAdvancedFilter filter)
    {
        var today = DateTime.Now.ToUniversalTime().Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        Query.Where(x => x.Name != null)
            .Where(x => x.Name!.Contains(filter.Keyword) || x.Description!.Contains(filter.Keyword) ||
                        x.Brand!.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
            .Where(x => x.Name!.Contains(filter.Name), !string.IsNullOrEmpty(filter.Name))
            .Where(x => x.Unit == filter.Unit, !string.IsNullOrEmpty(filter.Unit))
            .Where(x => x.Brand == filter.Brand, !string.IsNullOrEmpty(filter.Brand))
            .Where(x => x.Price <= filter.MaxPrice, filter.MaxPrice is not null)
            .Where(x => x.Price >= filter.MinPrice, filter.MinPrice is not null)
            .Where(x => x.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ProductListView.My)
            .Where(x => x.Created >= start && x.Created <= end, filter.ListView == ProductListView.CreatedToday)
            .Where(x => x.Created >= last30day, filter.ListView == ProductListView.Created30Days);
    }
}