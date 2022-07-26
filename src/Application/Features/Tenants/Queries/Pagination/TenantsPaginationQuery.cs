// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Queries.Pagination;

    public class TenantsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<TenantDto>>, ICacheable
    {
        public string CacheKey => TenantCacheKey.GetPaginationCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => TenantCacheKey.MemoryCacheEntryOptions;
    }
    
    public class TenantsWithPaginationQueryHandler :
         IRequestHandler<TenantsWithPaginationQuery, PaginatedData<TenantDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<TenantsWithPaginationQueryHandler> _localizer;

        public TenantsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<TenantsWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<TenantDto>> Handle(TenantsWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Tenants.Where(x=>x.Name.Contains(request.Keyword))
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
   }