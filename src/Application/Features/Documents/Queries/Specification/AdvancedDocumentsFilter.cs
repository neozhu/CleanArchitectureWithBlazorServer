namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.Specification;

public enum DocumentListView
{
    [Description("All")] All,
    [Description("My Document")] My,
    [Description("Created Toady")] CreatedToday
}
public class AdvancedDocumentsFilter: PaginationFilter
{
    public DocumentListView ListView { get; set; } = DocumentListView.All;
    public required UserProfile CurrentUser { get; set; }
}
