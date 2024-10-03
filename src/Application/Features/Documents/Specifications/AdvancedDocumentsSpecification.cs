namespace CleanArchitecture.Blazor.Application.Features.Documents.Specifications;
#nullable disable warnings
public class AdvancedDocumentsSpecification : Specification<Document>
{
    public AdvancedDocumentsSpecification(AdvancedDocumentsFilter filter)
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