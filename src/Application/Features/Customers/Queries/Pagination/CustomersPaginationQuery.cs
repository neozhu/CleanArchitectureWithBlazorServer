// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;

public class CustomersWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<CustomerDto>>
{
    [CompareTo("Name", "Description")] // <-- This filter will be applied to Name or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    public override string ToString()
    {
        return $"Search:{Keyword},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }
    public string CacheKey => CustomerCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => CustomerCacheKey.MemoryCacheEntryOptions;
}
    
public class CustomersWithPaginationQueryHandler :
         IRequestHandler<CustomersWithPaginationQuery, PaginatedData<CustomerDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CustomersWithPaginationQueryHandler> _localizer;

        public CustomersWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<CustomersWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<CustomerDto>> Handle(CustomersWithPaginationQuery request, CancellationToken cancellationToken)
        {
           // TODO: Implement CustomersWithPaginationQueryHandler method 
           var data = await _context.Customers.ApplyFilterWithoutPagination(request)
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.Page, request.PerPage);
            return data;
        }
}

public class CustomersPaginationSpecification : Specification<Customer>
{
    public CustomersPaginationSpecification(CustomersWithPaginationQuery query)
    {
        Criteria = q => q.Name != null;
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            And(x => x.Name.Contains(query.Keyword));
        }
       
    }
}