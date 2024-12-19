
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.AddEdit;

public class AddEditInvoiceLineCommand: ICacheInvalidatorRequest<Result<int>>
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

public class AddEditInvoiceLineCommandHandler : IRequestHandler<AddEditInvoiceLineCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public AddEditInvoiceLineCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(AddEditInvoiceLineCommand request, CancellationToken cancellationToken)
    {
   //     if (request.Id > 0)
   //     {
   //         var item = await _context.InvoiceLines.FindAsync(request.Id, cancellationToken);
   //         if (item == null)
   //         {
   //             return await Result<int>.FailureAsync($"InvoiceLine with id: [{request.Id}] not found.");
   //         }
   //         InvoiceLineMapper.ApplyChangesFrom(request,item);
			//// raise a update domain event
			//item.AddDomainEvent(new InvoiceLineUpdatedEvent(item));
   //         await _context.SaveChangesAsync(cancellationToken);
   //         return await Result<int>.SuccessAsync(item.Id);
   //     }
   //     else
   //     {
   //         var item = InvoiceLineMapper.FromEditCommand(request);
   //         // raise a create domain event
			//item.AddDomainEvent(new InvoiceLineCreatedEvent(item));
   //         _context.InvoiceLines.Add(item);
   //         await _context.SaveChangesAsync(cancellationToken);
   //         return await Result<int>.SuccessAsync(item.Id);
   //     }


        return await Result<int>.SuccessAsync(0);
    }
}

