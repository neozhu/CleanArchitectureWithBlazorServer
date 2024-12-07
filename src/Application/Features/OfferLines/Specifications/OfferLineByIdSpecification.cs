

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering OfferLines by their ID.
/// </summary>
public class OfferLineByIdSpecification : Specification<OfferLine>
{
    public OfferLineByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}