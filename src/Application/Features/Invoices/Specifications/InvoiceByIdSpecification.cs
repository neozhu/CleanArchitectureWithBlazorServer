namespace CleanArchitecture.Blazor.Application.Features.Invoices.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering Invoices by their ID.
/// </summary>
public class InvoiceByIdSpecification : Specification<Invoice>
{
    public InvoiceByIdSpecification(int id)
    {
        Query.Where(q => q.Id == id);
    }
}