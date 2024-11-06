using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Mappers;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _context;
    private readonly IFusionCache _fusionCache;

    public TenantService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _fusionCache = fusionCache;
    }

    public event Func<Task>? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();


    public void Initialize()
    {
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo()
                .ToList()) ?? new List<TenantDto>();
    }

    public void Refresh()
    {
        _fusionCache.Remove(TenantCacheKey.TenantsCacheKey);
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo()
                .ToList()) ?? new List<TenantDto>();
        OnChange?.Invoke();
    }
}