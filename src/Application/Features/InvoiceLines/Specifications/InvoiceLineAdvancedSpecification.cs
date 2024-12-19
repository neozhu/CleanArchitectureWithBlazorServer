

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for advanced filtering of InvoiceLines.
/// </summary>
public class InvoiceLineAdvancedSpecification : Specification<InvoiceLine>
{
    public InvoiceLineAdvancedSpecification(InvoiceLineAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(InvoiceLineListView.TODAY.ToString(), filter.LocalTimezoneOffset);
        var last30daysrange = today.GetDateRange(InvoiceLineListView.LAST_30_DAYS.ToString(),filter.LocalTimezoneOffset);

        Query
             .Where(filter.Keyword,!string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == InvoiceLineListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == InvoiceLineListView.TODAY)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == InvoiceLineListView.LAST_30_DAYS);
       
    }
}
