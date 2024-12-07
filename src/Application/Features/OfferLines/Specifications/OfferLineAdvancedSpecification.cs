

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for advanced filtering of OfferLines.
/// </summary>
public class OfferLineAdvancedSpecification : Specification<OfferLine>
{
    public OfferLineAdvancedSpecification(OfferLineAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(OfferLineListView.TODAY.ToString(), filter.LocalTimezoneOffset);
        var last30daysrange = today.GetDateRange(OfferLineListView.LAST_30_DAYS.ToString(),filter.LocalTimezoneOffset);

        Query.Where(q => q.Id != 0)
             .Where(filter.Keyword,!string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == OfferLineListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == OfferLineListView.TODAY)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == OfferLineListView.LAST_30_DAYS);
       
    }
}
