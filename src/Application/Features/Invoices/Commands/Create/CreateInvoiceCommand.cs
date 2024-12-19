using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
using CleanArchitecture.Blazor.Application.Invoices.Mappers;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Create;

public class CreateInvoiceCommand : ICacheInvalidatorRequest<Result<int>>
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

    public string CacheKey => InvoiceCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public CreateInvoiceCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var item = InvoiceMapper.FromCreateCommand(request);
        // raise a create domain event
        //item.AddDomainEvent(new InvoiceCreatedEvent(item));
        _context.Invoices.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}

