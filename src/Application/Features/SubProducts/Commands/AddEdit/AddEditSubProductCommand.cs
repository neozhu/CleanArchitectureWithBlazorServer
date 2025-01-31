

using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.SubProducts.Caching;
using CleanArchitecture.Blazor.Application.Features.SubProducts.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.SubProducts.Commands.AddEdit;

public class AddEditSubProductCommand: ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Prod id")]
    public int ProductId { get;set;} 
    [Description("Unit")]
    public string? Unit {get;set;} 
    [Description("Color")]
    public string? Color {get;set;}

    [Description("Price")]
    public decimal Price { get; set; }

    [Description("RetailPrice")]
    public decimal RetailPrice { get; set; }


    [Description("Product")]
    public ProductDto Product {get;set;} 


      public string CacheKey => SubProductCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => SubProductCacheKey.Tags;
}

public class AddEditSubProductCommandHandler : IRequestHandler<AddEditSubProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public AddEditSubProductCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(AddEditSubProductCommand request, CancellationToken cancellationToken)
    {
        request.ProductId = request.Product.Id; 
        request.Product = null;;

        try
        {
            if (request.Id > 0)
            {
                var item = await _context.SubProducts.FindAsync(request.Id, cancellationToken);
                if (item == null)
                {
                    return await Result<int>.FailureAsync($"SubProduct with id: [{request.Id}] not found.");
                }
                SubProductMapper.ApplyChangesFrom(request, item);
                // raise a update domain event
                //item.AddDomainEvent(new SubProductUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = SubProductMapper.FromEditCommand(request);
                // raise a create domain event
                //item.AddDomainEvent(new SubProductCreatedEvent(item));
                _context.SubProducts.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
        }
        catch (Exception ex)
        {
            var m = ex;
            throw;
        }

       
    }
}

