using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PicklistService : IPicklistService
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;

    public PicklistService(
        IMapper mapper,
        IFusionCache fusionCache,
        IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _fusionCache = fusionCache;
    }

    public event  Func<Task>? OnChange;
    public List<PicklistSetDto> DataSource { get; private set; } = new();


    public async Task InitializeAsync()
    {
        await using var db = await _dbContextFactory.CreateAsync();
        DataSource = _fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => db.PicklistSets.ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ToList()
        ) ?? new List<PicklistSetDto>();
    }

    public async Task RefreshAsync()
    {
        _fusionCache.Remove(PicklistSetCacheKey.PicklistCacheKey);
        await using var db = await _dbContextFactory.CreateAsync();
        DataSource = _fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => db.PicklistSets.ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ToList()
        ) ?? new List<PicklistSetDto>();
        if (OnChange != null) await OnChange.Invoke();
    }
}