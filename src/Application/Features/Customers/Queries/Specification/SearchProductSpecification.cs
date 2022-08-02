using CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Specification;

public class SearchCustomerSpecification : Specification<Customer>
{
    public SearchCustomerSpecification(CustomersWithPaginationQuery query)
    {
        Criteria = q => q.Name != null;
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            And(x => x.Name.Contains(query.Keyword) || x.Description.Contains(query.Keyword));
        }
    }
}
