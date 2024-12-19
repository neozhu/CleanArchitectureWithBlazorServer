
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Create;

public class CreateInvoiceLineCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Invoice id")]
    public int InvoiceId {get;set;} 
    [Description("Product id")]
    public int ProductId {get;set;} 
    [Description("Quantity")]
    public int Quantity {get;set;} 
    [Description("Unit price")]
    public decimal UnitPrice {get;set;} 
    [Description("Line total")]
    public decimal LineTotal {get;set;} 
    [Description("Discount")]
    public decimal Discount {get;set;} 
    [Description("Product")]
    public ProductDto Product {get;set;} 

      public string CacheKey => InvoiceLineCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
}
    
    public class CreateInvoiceLineCommandHandler : IRequestHandler<CreateInvoiceLineCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateInvoiceLineCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(CreateInvoiceLineCommand request, CancellationToken cancellationToken)
        {
        //   var item = InvoiceLineMapper.FromCreateCommand(request);
        //   // raise a create domain event
        //item.AddDomainEvent(new InvoiceLineCreatedEvent(item));
        //   _context.InvoiceLines.Add(item);
        //   await _context.SaveChangesAsync(cancellationToken);
        //   return  await Result<int>.SuccessAsync(item.Id);

            return await Result<int>.SuccessAsync(0);
        }
    }

