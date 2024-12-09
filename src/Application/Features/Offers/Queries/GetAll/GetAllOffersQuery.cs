
using CleanArchitecture.Blazor.Application.Features.Offers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Offers.Mappers;
using CleanArchitecture.Blazor.Application.Features.Offers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Offers.Queries.GetAll;

public class GetAllOffersQuery : ICacheableRequest<IEnumerable<OfferDto>>
{
   public string CacheKey => OfferCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => OfferCacheKey.Tags;
}

public class GetAllOffersQueryHandler :
     IRequestHandler<GetAllOffersQuery, IEnumerable<OfferDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllOffersQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OfferDto>> Handle(GetAllOffersQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Offers
                                        .Include(x=>x.OfferLines)
                                        .ProjectTo()
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);
        return data;
    }
}


