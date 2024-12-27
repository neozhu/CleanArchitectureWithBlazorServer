
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

public class GetSupplyItemByIdQueryHandler(
    IApplicationDbContext context) :
     IRequestHandler<GetSupplyItemByIdQuery, Result<SupplyItemDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<SupplyItemDto>> Handle(GetSupplyItemByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.SupplyItems
                        .AsNoTracking()
                        .Include( x => x.Product )
                        .Include(x=>x.Supplier)
                        .ApplySpecification(new SupplyItemByIdSpecification(request.Id))
                                                .ProjectTo()
                                                .FirstAsync(cancellationToken);

        return await Result<SupplyItemDto>.SuccessAsync(data);
    }
}


public class GetSupplyItemByProductIdQuery : ICacheableRequest<Result<SupplyItemDto>>
{
    public required int ProductId { get; set; }
    public string CacheKey => SupplyItemCacheKey.GetByIdCacheKey($"{ProductId}");
    public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
}

public class GetSupplyItemByProductIdHandler(
    IApplicationDbContext context) :
     IRequestHandler<GetSupplyItemByProductIdQuery, Result<SupplyItemDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<SupplyItemDto>> Handle(GetSupplyItemByProductIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.SupplyItems
                        .Where(x => x.ProductId == request.ProductId)
                        .AsNoTracking()
                        .Include(x => x.Product)
                        .Include(x => x.Supplier)
                                                .ProjectTo()
                                                .FirstAsync(cancellationToken);

        return await Result<SupplyItemDto>.SuccessAsync(data);
    }
}