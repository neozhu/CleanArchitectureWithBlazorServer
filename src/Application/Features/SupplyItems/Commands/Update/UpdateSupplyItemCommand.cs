
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Update;

public class UpdateSupplyItemCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Product id")]
    public int ProductId { get; set; }
    [Description("Supplier id")]
    public int SupplierId { get; set; }
    [Description("Quantity")]
    public int Quantity { get; set; }
    [Description("Cost per item")]
    public decimal CostPerItem { get; set; }
    [Description("Notes")]
    public string? Notes { get; set; }
    [Description("Product")]
    public ProductDto Product { get; set; }
    [Description("Supplier")]
    public SupplierDto Supplier { get; set; }

    public string CacheKey => SupplyItemCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;

}

public class UpdateSupplyItemCommandHandler : IRequestHandler<UpdateSupplyItemCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public UpdateSupplyItemCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(UpdateSupplyItemCommand request, CancellationToken cancellationToken)
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
}

