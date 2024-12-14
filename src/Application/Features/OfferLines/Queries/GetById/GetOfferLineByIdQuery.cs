

using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Queries.GetById;

public class GetOfferLineByIdQuery : ICacheableRequest<Result<OfferLineDto>>
{
   public required int Id { get; set; }

    public required int OfferId { get; set; }
    public string CacheKey => OfferLineCacheKey.GetByIdCacheKey($"{Id}");
   public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
}

public class GetOfferLineByIdQueryHandler :
     IRequestHandler<GetOfferLineByIdQuery, Result<OfferLineDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOfferLineByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OfferLineDto>> Handle(GetOfferLineByIdQuery request, CancellationToken cancellationToken)
    {
        //var data = await _context.OfferLines.ApplySpecification(new OfferLineByIdSpecification(request.Id))
        //                                        .ProjectTo()
        //                                        .FirstAsync(cancellationToken);
        try
        {
            //var offer = _context.Offers.FirstOrDefault(x => x.Id == request.OfferId);
            //if (offer == null)
            //    return await Result<OfferLineDto>.FailureAsync($"No items were find for Speficific offer wih id  {request}");

            //var sql = _context.Offers.Where(o => o.Id == request.OfferId)
            //    .SelectMany(o => o.OfferLines)
            //    .AsNoTracking()
            //    .Where(ol => ol.Id == request.Id)
            //    .ToQueryString();

            var data = await _context.Offers
                .Where(o => o.Id == request.OfferId)
                .SelectMany(o => o.OfferLines)
                .Where(ol => ol.Id == request.Id)
                .AsNoTracking()
                .ProjectTo()
                .FirstOrDefaultAsync(cancellationToken);

            //var data = await _context.Offers.Where(x=>x.Id == request.OfferId)
            //     .SelectMany(o => o.OfferLines)
            //     .AsNoTracking()
            //     .ApplySpecification(new OfferLineByIdSpecification(request.Id))
            //     .ProjectTo()
            //     .FirstAsync(cancellationToken);

            return await Result<OfferLineDto>.SuccessAsync(data);
        }
        catch (Exception ex)
        {
            var message = ex.Message;

            throw;
        }

    }
}
