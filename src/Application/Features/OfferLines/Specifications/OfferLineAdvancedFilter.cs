namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;

#nullable disable warnings
/// <summary>
/// Specifies the different views available for the OfferLine list.
/// </summary>
public enum OfferLineListView
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
/// A class for applying advanced filtering options to OfferLine lists.
/// </summary>
public class OfferLineAdvancedFilter: PaginationFilter
{
    public TimeSpan LocalTimezoneOffset { get; set; }
    public OfferLineListView ListView { get; set; } = OfferLineListView.All;
    public UserProfile? CurrentUser { get; set; }
}