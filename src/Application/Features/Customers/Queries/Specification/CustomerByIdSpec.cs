namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Specification;

public class CustomerByIdSpec : Specification<Customer>
{
    public CustomerByIdSpec(int id)
    {
       Query.Where(q => q.Id == id);
    }
}