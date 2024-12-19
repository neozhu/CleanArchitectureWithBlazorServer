namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Specifications;

#nullable disable warnings
/// <summary>
/// Specifies the different views available for the InvoiceLine list.
/// </summary>
public enum InvoiceLineListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Toady")]
    TODAY,
    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}
/// <summary>
/// A class for applying advanced filtering options to InvoiceLine lists.
/// </summary>
public class InvoiceLineAdvancedFilter: PaginationFilter
{
    public TimeSpan LocalTimezoneOffset { get; set; }
    public InvoiceLineListView ListView { get; set; } = InvoiceLineListView.All;
    public UserProfile? CurrentUser { get; set; }
}