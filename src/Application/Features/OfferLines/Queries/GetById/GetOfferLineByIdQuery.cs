

using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Queries.GetById;

public class GetOfferLineByIdQuery : ICacheableRequest<Result<OfferLineDto>>
{
   public required int Id { get; set; }
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
        var data = await _context.OfferLines.ApplySpecification(new OfferLineByIdSpecification(request.Id))
                                                .ProjectTo()
                                                .FirstAsync(cancellationToken);
        return await Result<OfferLineDto>.SuccessAsync(data);
    }
}
