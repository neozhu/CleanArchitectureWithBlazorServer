namespace CleanArchitecture.Blazor.Application.Features.Customers.Specifications;

public class CustomerByIdSpec : Specification<Customer>
{
    public CustomerByIdSpec(int id)
    {
       Query.Where(q => q.Id == id);
    }
}