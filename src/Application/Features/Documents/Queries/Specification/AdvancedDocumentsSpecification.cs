namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.Specification;
 
public class AdvancedDocumentsSpecification : Specification<Document>
{
    public AdvancedDocumentsSpecification(AdvancedDocumentsFilter filter)
    {
        Query.Where(p =>
                (p.CreatedBy == filter.CurrentUser.UserId && p.IsPublic == false) ||
                (p.IsPublic == true && p.TenantId == filter.CurrentUser.TenantId), filter.ListView == DocumentListView.All)
             .Where(p =>
                p.CreatedBy == filter.CurrentUser.UserId && p.TenantId == filter.CurrentUser.TenantId, filter.ListView == DocumentListView.My)
             .Where(p => p.Created.Value.Date == DateTime.Now.Date, filter.ListView == DocumentListView.CreatedToday)
             .Where(x => x.Title.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword) || x.Content.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));

    }
}
