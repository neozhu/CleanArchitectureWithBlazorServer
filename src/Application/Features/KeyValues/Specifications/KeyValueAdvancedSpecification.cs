namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Specifications;
#nullable disable warnings
public class KeyValueAdvancedSpecification : Specification<KeyValue>
{
    public KeyValueAdvancedSpecification(KeyValueAdvancedFilter filter)
    {
        Query.Where(p => p.Name == filter.Picklist, filter.Picklist is not null)
            .Where(
                x => x.Description.Contains(filter.Keyword) || x.Text.Contains(filter.Keyword) ||
                     x.Value.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}