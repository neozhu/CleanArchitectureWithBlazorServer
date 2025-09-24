
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;

    public TenantService(
        IMapper mapper,
        IFusionCache fusionCache,
        IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _fusionCache = fusionCache;
    }

    public event Func<Task>? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();


    public async Task InitializeAsync()
    {
        await using var db = await _dbContextFactory.CreateAsync();
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => db.Tenants.ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.Name)
                .ToList()) ?? new List<TenantDto>();
    }

    public async Task RefreshAsync()
    {
        _fusionCache.Remove(TenantCacheKey.TenantsCacheKey);
        await using var db = await _dbContextFactory.CreateAsync();
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => db.Tenants.ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.Name)
                .ToList()) ?? new List<TenantDto>();
        if (OnChange != null) await OnChange.Invoke();
    }
}
