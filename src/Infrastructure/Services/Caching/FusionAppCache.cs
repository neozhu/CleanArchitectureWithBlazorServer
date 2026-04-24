using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Caching;

public class FusionAppCache : IAppCache
{
    private readonly IFusionCache _cache;

    public FusionAppCache(IFusionCache cache)
    {
        _cache = cache;
    }

    public Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        return _cache.GetOrSetAsync(
            key,
            _ => factory(cancellationToken),
            tags: tags).AsTask();
    }

    public void Remove(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            _cache.Remove(key);
        }
    }

    public Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        return string.IsNullOrWhiteSpace(tag)
            ? Task.CompletedTask
            : _cache.RemoveByTagAsync(tag, token: cancellationToken).AsTask();
    }

    public async Task RemoveByTagsAsync(IEnumerable<string>? tags, CancellationToken cancellationToken = default)
    {
        if (tags is null)
        {
            return;
        }

        foreach (var tag in tags.Where(static x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.Ordinal))
        {
            await _cache.RemoveByTagAsync(tag, token: cancellationToken);
        }
    }
}
