// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Queries.Pagination;

public class TenantsWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<TenantDto>>
{
    public TenantsPaginationSpecification Specification => new(this);
    public string CacheKey => TenantCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => TenantCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class TenantsWithPaginationQueryHandler :
    IRequestHandler<TenantsWithPaginationQuery, PaginatedData<TenantDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<TenantsWithPaginationQueryHandler> _localizer;
    private readonly IMapper _mapper;

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

    public async Task<PaginatedData<TenantDto>> Handle(TenantsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.Tenants.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<Tenant, TenantDto>(request.Specification, request.PageNumber, request.PageSize,
                _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}
#nullable disable warnings
public class TenantsPaginationSpecification : Specification<Tenant>
{
    public TenantsPaginationSpecification(TenantsWithPaginationQuery query)
    {
        Query.Where(q => q.Name != null)
            .Where(q => q.Name.Contains(query.Keyword) || q.Description.Contains(query.Keyword),
                !string.IsNullOrEmpty(query.Keyword));
    }
}