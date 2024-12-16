

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering SupplyItems by their ID.
/// </summary>
public class SupplyItemByIdSpecification : Specification<SupplyItem>
{
    public SupplyItemByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}