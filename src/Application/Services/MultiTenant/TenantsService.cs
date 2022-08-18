using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using LazyCache;

namespace CleanArchitecture.Blazor.Application.Services.MultiTenant;

public class TenantsService : ITenantsService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IAppCache _cache;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
   
    public event Action? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();

    public TenantsService(
      IAppCache  cache,  
      IApplicationDbContext context, IMapper mapper)
    {
        _cache = cache;
        _context = context;
        _mapper = mapper;
    }
    public async Task Initialize()
    {
        //if (DataSource.Count > 0) return;
        await _semaphore.WaitAsync();
        try
        {
            DataSource = await _cache.GetOrAddAsync(TenantCacheKey.TenantsCacheKey,
                () => _context.Tenants.OrderBy(x => x.Name)
                    .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(),
                  TenantCacheKey.MemoryCacheEntryOptions);

        }
        finally
        {
            _semaphore.Release();
        }
       
    }
    public async Task Refresh()
    {
        await _semaphore.WaitAsync();
        try
        {
            _cache.Remove(TenantCacheKey.TenantsCacheKey);
            DataSource = await _cache.GetOrAddAsync(TenantCacheKey.TenantsCacheKey,
                () => _context.Tenants.OrderBy(x => x.Name)
                    .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(),
                  TenantCacheKey.MemoryCacheEntryOptions);
     
            OnChange?.Invoke();
        }
        finally
        {
            _semaphore.Release();
        }
       
    }
}
