namespace CleanArchitecture.Blazor.Application.Features.Documents.Specifications;
#nullable disable warnings
public class AdvancedDocumentsSpecification : Specification<Document>
{
    public AdvancedDocumentsSpecification(AdvancedDocumentsFilter filter)
    {
        var today = DateTime.Now.ToUniversalTime().Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        Query.Where(p =>
                    (p.CreatedBy == filter.CurrentUser.UserId && p.IsPublic == false) ||
                    (p.IsPublic == true && p.TenantId == filter.CurrentUser.TenantId),
                filter.ListView == DocumentListView.All)
            .Where(p =>
                    p.CreatedBy == filter.CurrentUser.UserId && p.TenantId == filter.CurrentUser.TenantId,
                filter.ListView == DocumentListView.My)
            .Where(q => q.Created >= start && q.Created <= end, filter.ListView == DocumentListView.CreatedToday)
            .Where(q => q.Created >= last30day, filter.ListView == DocumentListView.Created30Days)
            .Where(
                x => x.Title.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword) ||
                     x.Content.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}