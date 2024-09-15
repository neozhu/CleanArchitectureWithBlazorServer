namespace CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for advanced filtering of Contacts.
/// </summary>
public class ContactAdvancedSpecification : Specification<Contact>
{
    public ContactAdvancedSpecification(ContactAdvancedFilter filter)
    {
        var timezoneOffset = filter.LocalTimezoneOffset;
        var utcNow = DateTime.UtcNow;
        var localNow = utcNow.Date.AddHours(timezoneOffset);
        var startOfTodayLocalAsUtc = localNow;
        var endOfTodayLocalAsUtc = localNow.AddDays(1);
        var startOfLast30DaysLocalAsUtc = localNow.AddDays(-30);

       Query.Where(q => q.Name != null)
             .Where(filter.Keyword,!string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ContactListView.My && filter.CurrentUser is not null)
             .Where(q => q.Created >= startOfTodayLocalAsUtc && q.Created <= endOfTodayLocalAsUtc, filter.ListView == ContactListView.CreatedToday)
             .Where(q => q.Created >= startOfLast30DaysLocalAsUtc, filter.ListView == ContactListView.Created30Days);
       
    }
}
