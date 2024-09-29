namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;
#nullable disable warnings
public class PicklistSetAdvancedSpecification : Specification<PicklistSet>
{
    public PicklistSetAdvancedSpecification(PicklistSetAdvancedFilter filter)
    {
        Query.Where(p => p.Name == filter.Picklist, filter.Picklist is not null)
            .Where(
                x => x.Description.Contains(filter.Keyword) || x.Text.Contains(filter.Keyword) ||
                     x.Value.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
    }
}