using CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;
#nullable disable warnings
public class AuditTrailAdvancedSpecification : Specification<AuditTrail>
{
    public AuditTrailAdvancedSpecification(AuditTrailsWithPaginationQuery filter)
    {
        var timezoneOffset = filter.TimezoneOffset;
        var utcNow = DateTime.UtcNow;
        var localNow = utcNow.Date.AddHours(timezoneOffset);
        var startOfTodayLocalAsUtc = localNow;
        var endOfTodayLocalAsUtc = localNow.AddDays(1);
        var startOfLast30DaysLocalAsUtc = localNow.AddDays(-30);

      

        Query.Where(p => p.AuditType == filter.AuditType, filter.AuditType is not null)
            .Where(p => p.UserId == filter.CurrentUser.UserId,
                filter.ListView == AuditTrailListView.My && filter.CurrentUser is not null)
            .Where(p => p.DateTime.Date >= startOfTodayLocalAsUtc && p.DateTime.Date < endOfTodayLocalAsUtc, filter.ListView == AuditTrailListView.CreatedToday)
            .Where(p => p.DateTime >= startOfLast30DaysLocalAsUtc, filter.ListView == AuditTrailListView.Last30days)
            .Where(x => x.TableName.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}