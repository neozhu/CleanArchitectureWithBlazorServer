
using CleanArchitecture.Blazor.Application.Features.SupplyItems.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Queries.GetAll;

public class GetAllSupplyItemsQuery : ICacheableRequest<IEnumerable<SupplyItemDto>>
{
   public string CacheKey => SupplyItemCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
}

public class GetAllSupplyItemsQueryHandler :
     IRequestHandler<GetAllSupplyItemsQuery, IEnumerable<SupplyItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSupplyItemsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SupplyItemDto>> Handle(GetAllSupplyItemsQuery request, CancellationToken cancellationToken)
    {
        //var data = await _context.SupplyItems.ProjectTo()
        //                                        .AsNoTracking()
        //                                        .ToListAsync(cancellationToken);
        //return data;

        return null;
    }
}


