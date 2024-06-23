namespace CleanArchitecture.Blazor.Application.Features.Customers.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering Customers by their ID.
/// </summary>
public class CustomerByIdSpecification : Specification<Customer>
{
    public CustomerByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}