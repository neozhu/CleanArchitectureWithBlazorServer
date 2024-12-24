

using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;


namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Delete;

public class DeleteInvoiceLineCommand:  ICacheInvalidatorRequest<Result<int>>
{
  public int[] Id {  get; }
  public int InvoiceId { get; }
  public string CacheKey => InvoiceLineCacheKey.GetAllCacheKey;
  public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
  public DeleteInvoiceLineCommand(int[] id , int invoiceId)
  {
    Id = id;
    InvoiceId = invoiceId;
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

        var invoice = await _context.Invoices
            .Include(o => o.InvoiceLines)
            .FirstOrDefaultAsync(x=> x.Id ==  request.InvoiceId, cancellationToken);

        if (invoice is null)
            return await Result<int>.FailureAsync($"Invoice with with id: [{request.InvoiceId}] not found.");

        var linesToDelete = invoice.InvoiceLines
            .Where(x => request.Id.Contains(x.Id))
            .ToList();

        if (!linesToDelete.Any())
            return await Result<int>.FailureAsync("No matching OfferLines found to delete.");

        linesToDelete.ForEach(line => invoice.InvoiceLines.Remove(line));

        var result = await _context.SaveChangesAsync(cancellationToken);

        return await Result<int>.SuccessAsync(0);


        //var offer = await _context.Offers
        // .Include(o => o.OfferLines)
        // .FirstOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        //if (offer is null)
        //    return await Result<int>.FailureAsync($"Offer with id: [{request.OfferId}] not found.");

        //var linesToDelete = offer.OfferLines
        //    .Where(x => request.Id.Contains(x.Id))
        //    .ToList();

        //if (!linesToDelete.Any())
        //    return await Result<int>.FailureAsync("No matching OfferLines found to delete.");

        //linesToDelete.ForEach(line => offer.OfferLines.Remove(line));

        //var result = await _context.SaveChangesAsync(cancellationToken);

        //return await Result<int>.SuccessAsync(result);

    }

}

