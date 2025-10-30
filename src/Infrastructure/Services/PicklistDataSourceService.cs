using System.Linq.Expressions;
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
    public override async Task<IEnumerable<PicklistSetDto>> SearchAsync(
        Expression<Func<PicklistSetDto, bool>>? predicate,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);

        IQueryable<PicklistSetDto> q = db.PicklistSets
            .AsNoTracking()
            .ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider);

        if (predicate is not null)
            q = q.Where(predicate);

        var take = limit ?? DefaultLimit;

        var list = await q
            .OrderBy(t => t.Name)     
            .Take(take)
            .ToListAsync(cancellationToken);

        return list.AsReadOnly();
    }
    protected override async Task<List<PicklistSetDto>?> LoadAsync(CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.PicklistSets.ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
            .OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ToListAsync(cancellationToken);
    }
}
