using CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;
#nullable disable warnings
public class AuditTrailAdvancedSpecification : Specification<AuditTrail>
{
    public AuditTrailAdvancedSpecification(AuditTrailsWithPaginationQuery filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange("TODAY", filter.CurrentUser.LocalTimeOffset);
        var last30daysrange = today.GetDateRange("LAST_30_DAYS",filter.CurrentUser.LocalTimeOffset);



        Query.Where(p => p.AuditType == filter.AuditType, filter.AuditType is not null)
            .Where(p => p.UserId == filter.CurrentUser.UserId,
                filter.ListView == AuditTrailListView.My && filter.CurrentUser is not null)
            .Where(x => x.DateTime >= todayrange.Start && x.DateTime < todayrange.End.AddDays(1), filter.ListView ==  AuditTrailListView.TODAY)
            .Where(x => x.DateTime >= last30daysrange.Start, filter.ListView == AuditTrailListView.LAST_30_DAYS)
            .Where(x => x.TableName.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}
