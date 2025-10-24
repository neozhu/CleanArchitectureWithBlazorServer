using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PicklistDataSourceService : DataSourceServiceBase<PicklistSetDto>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public PicklistDataSourceService(
        IMapper mapper,
        IFusionCache fusionCache,
        IApplicationDbContextFactory dbContextFactory)
        : base(fusionCache, PicklistSetCacheKey.PicklistCacheKey)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    protected override async Task<List<PicklistSetDto>?> LoadAsync(CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.PicklistSets.ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
            .OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ToListAsync(cancellationToken);
    }
}
