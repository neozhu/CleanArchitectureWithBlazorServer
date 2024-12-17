

using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.AddEdit;

public class AddEditSupplyItemCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Product id")]
    public int ProductId {get;set;} 
    [Description("Supplier id")]
    public int SupplierId {get;set;}
    [Description("Quantity")]
    public int Quantity { get; set; } = 1;
    [Description("Cost per item")]
    public decimal CostPerItem {get;set;} 
    [Description("Notes")]
    public string? Notes {get;set;} 

    [Description("Product")]
    [MapperIgnore]
    public ProductDto Product {get;set;} 
    [Description("Supplier")]
    [MapperIgnore]
    public SupplierDto Supplier {get;set;} 


      public string CacheKey => SupplyItemCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
}

public class AddEditSupplyItemCommandHandler : IRequestHandler<AddEditSupplyItemCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public AddEditSupplyItemCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(AddEditSupplyItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id > 0)
            {
                var item = await _context.SupplyItems.FindAsync(request.Id, cancellationToken);
                if (item is null)
                {
                    return await Result<int>.FailureAsync($"SupplyItem with id: [{request.Id}] not found.");
                }
                SupplyItemMapper.ApplyChangesFrom(request, item);

                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = SupplyItemMapper.FromEditCommand(request);

                _context.SupplyItems.Add(item);
                
                await _context.SaveChangesAsync(cancellationToken);

                return await Result<int>.SuccessAsync(item.Id);
            }
        }
        catch (Exception ex)
        {
            var message = ex.ToString();
            throw;
        }


    }
}

