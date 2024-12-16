
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Delete;

public class DeleteSupplyItemCommand:  ICacheInvalidatorRequest<Result<int>>
{
  public int[] Id {  get; }
  public string CacheKey => SupplyItemCacheKey.GetAllCacheKey;
  public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
  public DeleteSupplyItemCommand(int[] id)
  {
    Id = id;
  }
}

public class DeleteSupplyItemCommandHandler : 
             IRequestHandler<DeleteSupplyItemCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    public DeleteSupplyItemCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(DeleteSupplyItemCommand request, CancellationToken cancellationToken)
    {
        //     var items = await _context.SupplyItems.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        //     foreach (var item in items)
        //     {
        //   // raise a delete domain event
        //item.AddDomainEvent(new SupplyItemDeletedEvent(item));
        //         _context.SupplyItems.Remove(item);
        //     }
        //     var result = await _context.SaveChangesAsync(cancellationToken);
        //     return await Result<int>.SuccessAsync(result);

        return await Result<int>.SuccessAsync(0);
    }

}

