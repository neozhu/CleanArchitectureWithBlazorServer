using CleanArchitecture.Blazor.Application.Features.Products.Queries.Export;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;

public class ExportProductSpecification : Specification<Product>
{
    public ExportProductSpecification(ExportProductsQuery query)
    {
        Criteria = q => q.Name != null;
        if (!string.IsNullOrEmpty(query.Keyword))
            And(x => x.Name!.Contains(query.Keyword) || x.Description!.Contains(query.Keyword) ||
                     x.Brand!.Contains(query.Keyword));
        if (!string.IsNullOrEmpty(query.Name)) And(x => x.Name!.Contains(query.Name));
        if (!string.IsNullOrEmpty(query.Unit)) And(x => x.Unit == query.Unit);
        if (!string.IsNullOrEmpty(query.Brand)) And(x => x.Brand == query.Brand);
        if (query.MaxPrice is not null) And(x => x.Price <= query.MaxPrice);
        if (query.MinPrice is not null) And(x => x.Price >= query.MinPrice);

        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        switch (query.ListView)
        {
            case ProductListView.My:
                And(product => product.CreatedBy == query.CurrentUser.UserId);
                break;
            case ProductListView.CreatedToday:
                And(product => product.Created >= start && product.Created <= end);
                break;
            case ProductListView.Created30Days:
                And(product => product.Created >= last30day);
                break;
            case ProductListView.All:
            default:
                break;
        }
    }
}
