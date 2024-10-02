using CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;
#nullable disable warnings
public class AuditTrailAdvancedSpecification : Specification<AuditTrail>
{
    public AuditTrailAdvancedSpecification(AuditTrailsWithPaginationQuery filter)
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



        Query.Where(p => p.AuditType == filter.AuditType, filter.AuditType is not null)
            .Where(p => p.UserId == filter.CurrentUser.UserId,
                filter.ListView == AuditTrailListView.My && filter.CurrentUser is not null)
            .Where(p => p.DateTime.Date >= startOfTodayLocalAsUtc && p.DateTime.Date < endOfTodayLocalAsUtc, filter.ListView == AuditTrailListView.CreatedToday)
            .Where(p => p.DateTime >= startOfLast30DaysLocalAsUtc, filter.ListView == AuditTrailListView.Last30days)
            .Where(x => x.TableName.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}