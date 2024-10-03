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

        Query.Where(q => q.Name != null)
             .Where(filter.Keyword,!string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == ContactListView.My && filter.CurrentUser is not null)
             .Where(q => q.Created >= startOfTodayLocalAsUtc && q.Created <= endOfTodayLocalAsUtc, filter.ListView == ContactListView.CreatedToday)
             .Where(q => q.Created >= startOfLast30DaysLocalAsUtc, filter.ListView == ContactListView.Created30Days);
       
    }
}
