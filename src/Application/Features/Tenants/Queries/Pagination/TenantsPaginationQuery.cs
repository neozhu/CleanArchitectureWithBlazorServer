// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Queries.Pagination;

public class TenantsWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<TenantDto>>
{
    [CompareTo("Id","Name", "Description")] // <-- This filter will be applied to Name or Brand or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    public override string ToString()
    {
        return $"Search:{Keyword},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }
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
        var data = await _context.Tenants.ApplyFilterWithoutPagination(request)
             .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
             .PaginatedDataAsync(request.Page, request.PerPage);
        return data;
    }
}