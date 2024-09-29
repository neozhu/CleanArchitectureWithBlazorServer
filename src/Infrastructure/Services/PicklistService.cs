using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PicklistService : IPicklistService
{
    private readonly IApplicationDbContext _context;
    private readonly IFusionCache _fusionCache;
    private readonly IMapper _mapper;

    public PicklistService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _fusionCache = fusionCache;
        _mapper = mapper;
    }

    public event  Func<Task>? OnChange;
    public List<PicklistSetDto> DataSource { get; private set; } = new();


    public void Initialize()
    {
        DataSource = _fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => _context.PicklistSets.OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
                .ToList()
        ) ?? new List<PicklistSetDto>();
    }

    public void Refresh()
    {
        _fusionCache.Remove(PicklistSetCacheKey.PicklistCacheKey);
        DataSource = _fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => _context.PicklistSets.OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
                .ToList()
        ) ?? new List<PicklistSetDto>();
        OnChange?.Invoke();
    }
}