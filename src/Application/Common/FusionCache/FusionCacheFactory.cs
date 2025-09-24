using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Application.Common.FusionCache;
public static class FusionCacheFactory
{
    private static IServiceProvider? _serviceProvider;

    public static void Configure(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static IFusionCache GetCache()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");

        return _serviceProvider.GetRequiredService<IFusionCache>();
    }
    public static void ClearCache()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");
        var cache = _serviceProvider.GetRequiredService<IFusionCache>();
        cache.Clear();
    }
    public static void RemoveByTags(IEnumerable<string>? tags)
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");
        var cache = _serviceProvider.GetRequiredService<IFusionCache>();
        if(tags is not null && tags.Any())
        {
            foreach(var tag in tags)
            {
                cache.RemoveByTag(tag);
            }
        }
    }
}
