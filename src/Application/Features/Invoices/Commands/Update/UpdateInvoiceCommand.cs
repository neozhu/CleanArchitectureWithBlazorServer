using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
using CleanArchitecture.Blazor.Application.Invoices.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Update;

public class UpdateInvoiceCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Offer id")]
    public int? OfferId { get; set; }
    [Description("Invoice date")]
    public DateTime InvoiceDate { get; set; }
    [Description("Total amount")]
    public decimal TotalAmount { get; set; }
    [Description("Status")]
    public string? Status { get; set; }
    [Description("Invoice lines")]
    public List<InvoiceLineDto>? InvoiceLines { get; set; }

    public string CacheKey => InvoiceCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;

}

public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public UpdateInvoiceCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {

        var item = await _context.Invoices.FindAsync(request.Id, cancellationToken);
        if (item == null)
        {
            return await Result<int>.FailureAsync($"Invoice with id: [{request.Id}] not found.");
        }
        InvoiceMapper.ApplyChangesFrom(request, item);
        // raise a update domain event
        //item.AddDomainEvent(new InvoiceUpdatedEvent(item));
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}

