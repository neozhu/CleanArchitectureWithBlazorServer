
using CleanArchitecture.Blazor.Application.Features.SupplyItems.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Queries.GetById;

public class GetSupplyItemByIdQuery : ICacheableRequest<Result<SupplyItemDto>>
{
   public required int Id { get; set; }
   public string CacheKey => SupplyItemCacheKey.GetByIdCacheKey($"{Id}");
   public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
}

public class GetSupplyItemByIdQueryHandler :
     IRequestHandler<GetSupplyItemByIdQuery, Result<SupplyItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSupplyItemByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SupplyItemDto>> Handle(GetSupplyItemByIdQuery request, CancellationToken cancellationToken)
    {
        //var data = await _context.SupplyItems.ApplySpecification(new SupplyItemByIdSpecification(request.Id))
        //                                        .ProjectTo()
        //                                        .FirstAsync(cancellationToken);
        //return await Result<SupplyItemDto>.SuccessAsync(data);

        return await Result<SupplyItemDto>.SuccessAsync(new SupplyItemDto());
    }
}
