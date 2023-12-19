namespace CleanArchitecture.Blazor.Application.Common.Models;

public class PaginationFilter : BaseFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "Descending";

    public override string ToString()
    {
        return
            $"PageNumber:{PageNumber},PageSize:{PageSize},OrderBy:{OrderBy},SortDirection:{SortDirection},Keyword:{Keyword}";
    }
}

public class BaseFilter
{
    public string? Keyword { get; set; }
}