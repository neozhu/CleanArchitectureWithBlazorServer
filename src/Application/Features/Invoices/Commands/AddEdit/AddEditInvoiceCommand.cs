using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
using CleanArchitecture.Blazor.Application.Invoices.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.AddEdit;

public class AddEditInvoiceCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Offer id")]
    public int? OfferId { get; set; }
    [Description("Invoice date")]
    public DateTime? InvoiceDate { get; set; }
    [Description("Total amount")]
    public decimal TotalAmount { get; set; }
    [Description("Status")]
    public string? Status { get; set; }
    [Description("Invoice lines")]
    public List<InvoiceLineDto>? InvoiceLines { get; set; }

    [Description("Supplier Id")]
    public int SupplierId { get; set; } 


    public string CacheKey => InvoiceCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;
}

public class AddEditInvoiceCommandHandler : IRequestHandler<AddEditInvoiceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public AddEditInvoiceCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(AddEditInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
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
        else
        {
            var item = InvoiceMapper.FromEditCommand(request);
            // raise a create domain event
            //item.AddDomainEvent(new InvoiceCreatedEvent(item));
            _context.Invoices.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }

    }
}

