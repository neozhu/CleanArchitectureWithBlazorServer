namespace CleanArchitecture.Blazor.Application.Features.Documents.Specifications;

public enum DocumentListView
{
    [Description("All")] All,
    [Description("My Document")] My,
    [Description("Created Today")] TODAY,

    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}

public class AdvancedDocumentsFilter : PaginationFilter
{
    public DocumentListView ListView { get; set; } = DocumentListView.All;
    public UserProfile? CurrentUser { get; set; }
}