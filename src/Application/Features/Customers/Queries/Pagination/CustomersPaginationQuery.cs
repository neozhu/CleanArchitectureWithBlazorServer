// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.Queries.Specification;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;

    public class CustomersWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<CustomerDto>>, ICacheable
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public override string ToString()
        {
            return $"{base.ToString()},Name:{Name},Description:{Description}";
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
           var data = await _context.Customers.Specify(new SearchCustomerSpecification(request))
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
   }