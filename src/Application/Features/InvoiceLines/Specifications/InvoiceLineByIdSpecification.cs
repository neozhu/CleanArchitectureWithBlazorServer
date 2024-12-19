
namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering InvoiceLines by their ID.
/// </summary>
public class InvoiceLineByIdSpecification : Specification<InvoiceLine>
{
    public InvoiceLineByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}