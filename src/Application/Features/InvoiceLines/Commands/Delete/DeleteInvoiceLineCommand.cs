

using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;


namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Delete;

public class DeleteInvoiceLineCommand:  ICacheInvalidatorRequest<Result<int>>
{
  public int[] Id {  get; }
  public string CacheKey => InvoiceLineCacheKey.GetAllCacheKey;
  public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
  public DeleteInvoiceLineCommand(int[] id)
  {
    Id = id;
  }
}

public class DeleteInvoiceLineCommandHandler : 
             IRequestHandler<DeleteInvoiceLineCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    public DeleteInvoiceLineCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(DeleteInvoiceLineCommand request, CancellationToken cancellationToken)
    {
        //     var items = await _context.InvoiceLines.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        //     foreach (var item in items)
        //     {
        //   // raise a delete domain event
        //item.AddDomainEvent(new InvoiceLineDeletedEvent(item));
        //         _context.InvoiceLines.Remove(item);
        //     }
        //     var result = await _context.SaveChangesAsync(cancellationToken);
        //     return await Result<int>.SuccessAsync(result);
        return await Result<int>.SuccessAsync(0);
    }

}

