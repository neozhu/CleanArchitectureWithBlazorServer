

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for advanced filtering of SupplyItems.
/// </summary>
public class SupplyItemAdvancedSpecification : Specification<SupplyItem>
{
    public SupplyItemAdvancedSpecification(SupplyItemAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(SupplyItemListView.TODAY.ToString(), filter.LocalTimezoneOffset);
        var last30daysrange = today.GetDateRange(SupplyItemListView.LAST_30_DAYS.ToString(),filter.LocalTimezoneOffset);

        Query
             .Where(filter.Keyword,!string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == SupplyItemListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == SupplyItemListView.TODAY)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == SupplyItemListView.LAST_30_DAYS);
       
    }
}
