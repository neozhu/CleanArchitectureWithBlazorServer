using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using LazyCache;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IAppCache _cache;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TenantService(
        IAppCache cache,
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _cache = cache;
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _mapper = mapper;
    }

    public event Action? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();

    public async Task InitializeAsync()
    {
        DataSource = await _cache.GetOrAddAsync(TenantCacheKey.TenantsCacheKey,
            () => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToListAsync(),
            TenantCacheKey.MemoryCacheEntryOptions);
    }

    public void Initialize()
    {
        DataSource = _cache.GetOrAdd(TenantCacheKey.TenantsCacheKey,
            () => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToList(),
            TenantCacheKey.MemoryCacheEntryOptions);
    }

    public async Task Refresh()
    {
        _cache.Remove(TenantCacheKey.TenantsCacheKey);
        DataSource = await _cache.GetOrAddAsync(TenantCacheKey.TenantsCacheKey,
            () => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToListAsync(),
            TenantCacheKey.MemoryCacheEntryOptions);
        OnChange?.Invoke();
    }
}