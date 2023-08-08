using CleanArchitecture.Blazor.Application.Features.Products.Queries.Export;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;

public class ExportProductSpecification : Specification<Product>
{
    public ExportProductSpecification(ExportProductsQuery query)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        Query.Where(q => q.Name != null)
             .Where(x => x.Name!.Contains(query.Keyword) || x.Description!.Contains(query.Keyword) ||
                     x.Brand!.Contains(query.Keyword), !string.IsNullOrEmpty(query.Keyword))
             .Where(x => x.Name!.Contains(query.Name), !string.IsNullOrEmpty(query.Name))
             .Where(x => x.Unit == query.Unit, !string.IsNullOrEmpty(query.Unit))
             .Where(x => x.Brand == query.Brand, !string.IsNullOrEmpty(query.Brand))
             .Where(x => x.Price <= query.MaxPrice, !string.IsNullOrEmpty(query.Brand))
             .Where(x => x.Price >= query.MinPrice, query.MinPrice is not null)
             .Where(product => product.CreatedBy == query.CurrentUser.UserId, query.ListView == ProductListView.My)
             .Where(product => product.Created >= start && product.Created <= end, query.ListView == ProductListView.CreatedToday)
             .Where(product => product.Created >= last30day, query.ListView == ProductListView.Created30Days);
    }
}
