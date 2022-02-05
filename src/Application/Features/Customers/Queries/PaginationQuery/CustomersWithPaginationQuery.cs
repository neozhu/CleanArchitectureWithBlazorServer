// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Customers.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using CleanArchitecture.Razor.Application.Features.Customers.Caching;

namespace CleanArchitecture.Razor.Application.Features.Customers.Queries.PaginationQuery;

public class CustomersWithPaginationQuery : PaginationRequest, IRequest<PaginatedData<CustomerDto>>
{
    public string CacheKey => CustomerCacheKey.GetPaginationCacheKey(this.ToString());

    public MemoryCacheEntryOptions Options => CustomerCacheKey.MemoryCacheEntryOptions;

}
public class CustomersWithPaginationQueryHandler : IRequestHandler<CustomersWithPaginationQuery, PaginatedData<CustomerDto>>
{

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CustomersWithPaginationQueryHandler(

        IApplicationDbContext context,
        IMapper mapper
        )
    {

        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedData<CustomerDto>> Handle(CustomersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<Customer>(request.FilterRules);
        var data = await _context.Customers.Where(filters)
            .OrderBy($"{request.Sort} {request.Order}")
            .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.Page, request.Rows);

        return data;
    }
}
