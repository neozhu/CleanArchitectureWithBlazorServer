namespace CleanArchitecture.Blazor.Application.Features.Documents.Specifications;
#nullable disable warnings
public class AdvancedDocumentsSpecification : Specification<Document>
{
    public AdvancedDocumentsSpecification(AdvancedDocumentsFilter filter)
    {
        var timezoneOffset = filter.LocalTimezoneOffset;
        var utcNow = DateTime.UtcNow;
        var localNow = utcNow.Date.AddHours(timezoneOffset);
        var startOfTodayLocalAsUtc = localNow;
        var endOfTodayLocalAsUtc = localNow.AddDays(1);
        var startOfLast30DaysLocalAsUtc = localNow.AddDays(-30);
        Query.Where(p =>
                    (p.CreatedBy == filter.CurrentUser.UserId && p.IsPublic == false) ||
                    (p.IsPublic == true && p.TenantId == filter.CurrentUser.TenantId),
                filter.ListView == DocumentListView.All)
            .Where(p =>
                    p.CreatedBy == filter.CurrentUser.UserId && p.TenantId == filter.CurrentUser.TenantId,
                filter.ListView == DocumentListView.My)
            .Where(q => q.Created >= startOfTodayLocalAsUtc && q.Created < endOfTodayLocalAsUtc, filter.ListView == DocumentListView.CreatedToday)
            .Where(q => q.Created >= startOfLast30DaysLocalAsUtc, filter.ListView == DocumentListView.Created30Days)
            .Where(
                x => x.Title.Contains(filter.Keyword) || x.Description.Contains(filter.Keyword) ||
                     x.Content.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}