namespace CleanArchitecture.Blazor.Application.Features.Customers.Specifications;
#nullable disable warnings
public class CustomerByIdSpecification : Specification<Customer>
{
    public CustomerByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}