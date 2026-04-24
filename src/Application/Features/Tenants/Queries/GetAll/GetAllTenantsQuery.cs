// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Mapster;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Queries.GetAll;

public class GetAllTenantsQuery : ICacheableRequest<IEnumerable<TenantDto>>
{
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => TenantCacheKey.Tags;
}

public class GetAllTenantsQueryHandler :
    IRequestHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
{
    private readonly TypeAdapterConfig _typeAdapterConfig;
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public GetAllTenantsQueryHandler(
        TypeAdapterConfig typeAdapterConfig,
       IApplicationDbContextFactory dbContextFactory
    )
    {
        _typeAdapterConfig = typeAdapterConfig;
        _dbContextFactory = dbContextFactory;
    }

    public async ValueTask<IEnumerable<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.Tenants
            .ProjectToType<TenantDto>(_typeAdapterConfig)
            .ToListAsync(cancellationToken);
        return data;
    }
}
