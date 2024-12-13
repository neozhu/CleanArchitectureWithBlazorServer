

// Usage:
// This command can be used to delete multiple OfferLines from the system by specifying
// their IDs. The handler also raises domain events for each deleted offerline to
// notify other bounded contexts and invalidate relevant cache entries.

using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;


namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Delete;

public class DeleteOfferLineCommand:  ICacheInvalidatorRequest<Result<int>>
{
  public int[] Id {  get; }
  public string CacheKey => OfferLineCacheKey.GetAllCacheKey;
  public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
  public DeleteOfferLineCommand(int[] id)
  {
    Id = id;
  }
}

public class DeleteOfferLineCommandHandler : 
             IRequestHandler<DeleteOfferLineCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    public DeleteOfferLineCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(DeleteOfferLineCommand request, CancellationToken cancellationToken)
    {
        //     var items = await _context.OfferLines.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        //     foreach (var item in items)
        //     {
        //   // raise a delete domain event
        //item.AddDomainEvent(new OfferLineDeletedEvent(item));
        //         _context.OfferLines.Remove(item);
        //     }
        //     var result = await _context.SaveChangesAsync(cancellationToken);
        //     return await Result<int>.SuccessAsync(result);

        return await Result<int>.SuccessAsync(0);
    }

}

