using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using LazyCache;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PicklistService : IPicklistService
{
    private readonly IAppCache _cache;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PicklistService(
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
    public List<KeyValueDto> DataSource { get; private set; } = new();

    public async Task InitializeAsync()
    {
        DataSource = await _cache.GetOrAddAsync(KeyValueCacheKey.PicklistCacheKey,
            () => _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
                .ToListAsync(),
            KeyValueCacheKey.MemoryCacheEntryOptions);
    }

    public void Initialize()
    {
        DataSource = _cache.GetOrAdd(KeyValueCacheKey.PicklistCacheKey,
            () => _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
                .ToList(),
            KeyValueCacheKey.MemoryCacheEntryOptions);
    }

    public async Task Refresh()
    {
        _cache.Remove(KeyValueCacheKey.PicklistCacheKey);
        DataSource = await _cache.GetOrAddAsync(KeyValueCacheKey.PicklistCacheKey,
            () => _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
                .ToListAsync(),
            KeyValueCacheKey.MemoryCacheEntryOptions
        );
        OnChange?.Invoke();
    }
}