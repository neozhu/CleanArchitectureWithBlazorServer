using CleanArchitecture.Blazor.Domain.Features.KeyValues;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Specifications;
public class KeyValueAdvancedFilter: PaginationFilter
{
    public Picklist? Picklist { get; set; }
}
