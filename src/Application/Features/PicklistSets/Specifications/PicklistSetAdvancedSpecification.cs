namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;
#nullable disable warnings
public class PicklistSetAdvancedSpecification : Specification<PicklistSet>
{
    public PicklistSetAdvancedSpecification(PicklistSetAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(PickListView.TODAY.ToString(), filter.CurrentUser.LocalTimeOffset);
        var last30daysrange = today.GetDateRange(PickListView.LAST_30_DAYS.ToString(), filter.CurrentUser.LocalTimeOffset);

        Query.Where(p => p.Name == filter.Picklist, filter.Picklist is not null)
             .Where(q => q.CreatedById == filter.CurrentUser.UserId, filter.ListView == PickListView.My && filter.CurrentUser is not null)
             .Where(x => x.CreatedAt >= todayrange.Start && x.CreatedAt < todayrange.End.AddDays(1), filter.ListView == PickListView.TODAY)
             .Where(x => x.CreatedAt >= last30daysrange.Start, filter.ListView == PickListView.LAST_30_DAYS)
             .Where(
                x => x.Description.Contains(filter.Keyword) || x.Text.Contains(filter.Keyword) ||
                     x.Value.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}
