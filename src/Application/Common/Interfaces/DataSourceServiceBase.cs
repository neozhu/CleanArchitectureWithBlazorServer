using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public abstract class DataSourceServiceBase<T> : IDataSourceService<T>
{
    private readonly IFusionCache _fusionCache;
    private readonly string _cacheKey;
    private readonly FusionCacheEntryOptions? _cacheOptions;
    protected List<T> Items { get; private set; } = new();

    protected DataSourceServiceBase(IFusionCache fusionCache, string cacheKey, FusionCacheEntryOptions? cacheOptions = null)
    {
        _fusionCache = fusionCache;
        _cacheKey = cacheKey;
        _cacheOptions = cacheOptions;
    }

    public IReadOnlyList<T> DataSource => Items;
    public event Func<Task>? OnChange;

    public async Task InitializeAsync()
    {
        if (Items.Count == 0)
        {
            await LoadAndCacheAsync();
            if (OnChange != null) await OnChange.Invoke();
        }
    }

    public async Task RefreshAsync()
    {
        _fusionCache.Remove(_cacheKey);
        await LoadAndCacheAsync();
        if (OnChange != null) await OnChange.Invoke();
    }

    private async Task LoadAndCacheAsync(CancellationToken cancellationToken = default)
    {
        var list = await GetOrSetAsync(_cacheKey, async () => await LoadAsync(cancellationToken));
        Items = list ?? new List<T>();
    }

    private async Task<List<T>?> GetOrSetAsync(string key, Func<Task<List<T>?>> factory)
    {
        if (_cacheOptions is null)
        {
            return await _fusionCache.GetOrSetAsync(key, async _ => await factory());
        }
        return await _fusionCache.GetOrSetAsync(key, async _ => await factory(), _cacheOptions);
    }

    protected abstract Task<List<T>?> LoadAsync(CancellationToken cancellationToken);
}
