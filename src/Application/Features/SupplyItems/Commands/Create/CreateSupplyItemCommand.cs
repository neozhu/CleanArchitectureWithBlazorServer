

using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Create;

public class CreateSupplyItemCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Product id")]
    public int ProductId {get;set;} 
    [Description("Supplier id")]
    public int SupplierId {get;set;} 
    [Description("Quantity")]
    public int Quantity {get;set;} 
    [Description("Cost per item")]
    public decimal CostPerItem {get;set;} 
    [Description("Notes")]
    public string? Notes {get;set;} 
    [Description("Product")]
    public ProductDto Product {get;set;} 
    [Description("Supplier")]
    public SupplierDto Supplier {get;set;} 

      public string CacheKey => SupplyItemCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
}
    
    public class CreateSupplyItemCommandHandler : IRequestHandler<CreateSupplyItemCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateSupplyItemCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(CreateSupplyItemCommand request, CancellationToken cancellationToken)
        {
            var item = SupplyItemMapper.FromCreateCommand(request);

            _context.SupplyItems.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            return await Result<int>.SuccessAsync(item.Id);

        }
    }

