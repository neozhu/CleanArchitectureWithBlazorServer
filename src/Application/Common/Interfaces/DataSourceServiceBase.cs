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
    protected int DefaultLimit { get; init; } = 20;
    private readonly Func<T, string> _textSelector;
    private readonly StringComparison _comparison=StringComparison.InvariantCultureIgnoreCase;
    protected List<T> Items { get; private set; } = new();

    protected DataSourceServiceBase(IFusionCache fusionCache, string cacheKey, FusionCacheEntryOptions? cacheOptions = null,Func<T, string>? textSelector = null)
    {
        _fusionCache = fusionCache;
        _cacheKey = cacheKey;
        _cacheOptions = cacheOptions;
        _textSelector = textSelector ?? (static _ => string.Empty);
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
    public virtual Task<IEnumerable<T>> SearchAsync(
        Expression<Func<T, bool>>? predicate,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var query = Items.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        var result = query.Take(limit??DefaultLimit).ToList();
        return Task.FromResult<IEnumerable<T>>(result);
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
