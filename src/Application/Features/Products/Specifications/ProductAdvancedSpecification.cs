namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;
#nullable disable warnings
public class ProductAdvancedSpecification : Specification<Product>
{
    public ProductAdvancedSpecification(ProductAdvancedFilter filter)
    {
        var timezoneOffset = filter.LocalTimezoneOffset;
        var utcNow = DateTime.UtcNow;
        var localNow = utcNow.Date.AddHours(timezoneOffset);
        var startOfTodayLocalAsUtc = localNow;
        var endOfTodayLocalAsUtc = localNow.AddDays(1);
        var startOfLast30DaysLocalAsUtc = localNow.AddDays(-30);
        Query.Where(x => x.Name != null)
            .Where(x => x.Name!.Contains(filter.Keyword) || x.Description!.Contains(filter.Keyword) ||
                        x.Brand!.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
            .Where(x => x.Name!.Contains(filter.Name), !string.IsNullOrEmpty(filter.Name))
            .Where(x => x.Unit == filter.Unit, !string.IsNullOrEmpty(filter.Unit))
            .Where(x => x.Brand == filter.Brand, !string.IsNullOrEmpty(filter.Brand))
            .Where(x => x.Price <= filter.MaxPrice, filter.MaxPrice is not null)
            .Where(x => x.Price >= filter.MinPrice, filter.MinPrice is not null)
            .Where(x => x.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ProductListView.My)
            .Where(x => x.Created >= startOfTodayLocalAsUtc && x.Created <= endOfTodayLocalAsUtc, filter.ListView == ProductListView.CreatedToday)
            .Where(x => x.Created >= startOfLast30DaysLocalAsUtc, filter.ListView == ProductListView.Created30Days);
    }
}