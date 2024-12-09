
using CleanArchitecture.Blazor.Application.Features.Offers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Offers.Caching;
using CleanArchitecture.Blazor.Application.Features.Offers.Mappers;
using CleanArchitecture.Blazor.Application.Features.Offers.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Offers.Queries.GetById;

public class GetOfferByIdQuery : ICacheableRequest<Result<OfferDto>>
{
   public required int Id { get; set; }
   public string CacheKey => OfferCacheKey.GetByIdCacheKey($"{Id}");
   public IEnumerable<string>? Tags => OfferCacheKey.Tags;
}

public class GetOfferByIdQueryHandler :
     IRequestHandler<GetOfferByIdQuery, Result<OfferDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOfferByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OfferDto>> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Offers.AsNoTracking()
                                                .ApplySpecification(new OfferByIdSpecification(request.Id))
                                                .ProjectTo()
                                                .FirstAsync(cancellationToken);

        return await Result<OfferDto>.SuccessAsync(data);
    }
}
