using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using DocumentFormat.OpenXml.InkML;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Queries.GetAll;

public class GetAllOfferLinesQuery : ICacheableRequest<IEnumerable<OfferLineDto>>
{
   public string CacheKey => OfferLineCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
}

public class GetAllOfferLinesQueryHandler :
     IRequestHandler<GetAllOfferLinesQuery, IEnumerable<OfferLineDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllOfferLinesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OfferLineDto>> Handle(GetAllOfferLinesQuery request, CancellationToken cancellationToken)
    {
        //var data1 = await _context.OfferLines.ProjectTo()
        //                                        .AsNoTracking()
        //.ToListAsync(cancellationToken);

        var data = await _context.Offers
                .SelectMany(o => o.OfferLines)
                .ProjectTo()
                .ToListAsync(cancellationToken);

        return data;
    }
}


