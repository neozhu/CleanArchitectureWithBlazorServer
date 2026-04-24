using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using Mapster;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public class TenantDataSourceService : DataSourceServiceBase<TenantDto>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly TypeAdapterConfig _typeAdapterConfig;

    public TenantDataSourceService(
        TypeAdapterConfig typeAdapterConfig,
        IFusionCache fusionCache,
        IApplicationDbContextFactory dbContextFactory)
        : base(fusionCache, TenantCacheKey.TenantsCacheKey)
    {
        _dbContextFactory = dbContextFactory;
        _typeAdapterConfig = typeAdapterConfig;
    }

    protected override async Task<List<TenantDto>?> LoadAsync(CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.Tenants.ProjectToType<TenantDto>(_typeAdapterConfig)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
