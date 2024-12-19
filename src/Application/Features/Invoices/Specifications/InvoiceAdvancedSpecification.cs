namespace CleanArchitecture.Blazor.Application.Features.Invoices.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for advanced filtering of Invoices.
/// </summary>
public class InvoiceAdvancedSpecification : Specification<Invoice>
{
    public InvoiceAdvancedSpecification(InvoiceAdvancedFilter filter)
    {
        var today = DateTime.UtcNow;
        var todayrange = today.GetDateRange(InvoiceListView.TODAY.ToString(), filter.LocalTimezoneOffset);
        var last30daysrange = today.GetDateRange(InvoiceListView.LAST_30_DAYS.ToString(), filter.LocalTimezoneOffset);

        Query
             .Where(filter.Keyword, !string.IsNullOrEmpty(filter.Keyword))
             .Where(q => q.CreatedBy == filter.CurrentUser.UserId, filter.ListView == InvoiceListView.My && filter.CurrentUser is not null)
             .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == InvoiceListView.TODAY)
             .Where(x => x.Created >= last30daysrange.Start, filter.ListView == InvoiceListView.LAST_30_DAYS);

    }
}
