using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.Specification;
public class KeyValueAdvancedFilter: PaginationFilter
{
    public Picklist? Picklist { get; set; }
}
