using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Delete;

public class DeleteInvoiceCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int[] Id { get; }
    public string CacheKey => InvoiceCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;
    public DeleteInvoiceCommand(int[] id)
    {
        Id = id;
    }
}

public class DeleteInvoiceCommandHandler :
             IRequestHandler<DeleteInvoiceCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    public DeleteInvoiceCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Invoices.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            // raise a delete domain event
            //item.AddDomainEvent(new InvoiceDeletedEvent(item));
            _context.Invoices.Remove(item);
        }
        var result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }

}

