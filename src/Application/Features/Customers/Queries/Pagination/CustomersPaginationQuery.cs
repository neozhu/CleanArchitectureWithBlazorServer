// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;

public class CustomersWithPaginationQuery : CustomerAdvancedFilter, ICacheableRequest<PaginatedData<CustomerDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => CustomerCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => CustomerCacheKey.MemoryCacheEntryOptions;
    public CustomerAdvancedSpecification Specification => new CustomerAdvancedSpecification(this);
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
           var data = await _context.Customers.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<Customer, CustomerDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}