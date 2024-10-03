namespace CleanArchitecture.Blazor.Application.Features.Products.Specifications;
#nullable disable warnings
public class ProductAdvancedSpecification : Specification<Product>
{
    public ProductAdvancedSpecification(ProductAdvancedFilter filter)
    {
        var timezoneOffset = filter.LocalTimezoneOffset;
        var utcNow = DateTime.UtcNow;
        // Corrected: Add the time zone offset to UTC time to get local time
        var localNow = utcNow.AddHours(timezoneOffset);

        // Calculate the start and end of today in local time
        var startOfTodayLocal = localNow.Date;
        var endOfTodayLocal = startOfTodayLocal.AddDays(1);
        var startOfLast30DaysLocal = startOfTodayLocal.AddDays(-30);

        // Convert local times back to UTC to match the TimeStamp's time zone
        var startOfTodayLocalAsUtc = startOfTodayLocal.AddHours(-timezoneOffset);
        var endOfTodayLocalAsUtc = endOfTodayLocal.AddHours(-timezoneOffset);
        var startOfLast30DaysLocalAsUtc = startOfLast30DaysLocal.AddHours(-timezoneOffset);
        Query.Where(x => x.Name != null)
            .Where(x => x.Name.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword) ||
                        x.Brand.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
            .Where(x => x.Name.Contains(filter.Name), !string.IsNullOrEmpty(filter.Name))
            .Where(x => x.Unit == filter.Unit, !string.IsNullOrEmpty(filter.Unit))
            .Where(x => x.Brand == filter.Brand, !string.IsNullOrEmpty(filter.Brand))
            .Where(x => x.Price <= filter.MaxPrice, filter.MaxPrice is not null)
            .Where(x => x.Price >= filter.MinPrice, filter.MinPrice is not null)
            .Where(x => x.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ProductListView.My)
            .Where(x => x.Created >= startOfTodayLocalAsUtc && x.Created <= endOfTodayLocalAsUtc, filter.ListView == ProductListView.CreatedToday)
            .Where(x => x.Created >= startOfLast30DaysLocalAsUtc, filter.ListView == ProductListView.Created30Days);
    }
}