using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Mappers;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PicklistService : IPicklistService
{
    private readonly IApplicationDbContext _context;
    private readonly IFusionCache _fusionCache;

    public PicklistService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _fusionCache = fusionCache;
    }

    public event  Func<Task>? OnChange;
    public List<PicklistSetDto> DataSource { get; private set; } = new();


    public void Initialize()
    {
        DataSource = _fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => _context.PicklistSets.OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ProjectTo()
                .ToList()
        ) ?? new List<PicklistSetDto>();
    }

    public void Refresh()
    {
        _fusionCache.Remove(PicklistSetCacheKey.PicklistCacheKey);
        DataSource = _fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => _context.PicklistSets.OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ProjectTo()
                .ToList()
        ) ?? new List<PicklistSetDto>();
        OnChange?.Invoke();
    }
}