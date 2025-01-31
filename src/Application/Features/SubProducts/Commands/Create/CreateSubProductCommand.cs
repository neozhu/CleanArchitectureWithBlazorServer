

using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.SubProducts.Caching;
using CleanArchitecture.Blazor.Application.Features.SubProducts.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.SubProducts.Commands.Create;

public class CreateSubProductCommand: ICacheInvalidatorRequest<Result<int>>
{
   [Description("Id")]
    public int Id { get; set; }
   [Description("Prod id")]
    public int ProdId {get;set;} 
    [Description("Unit")]
    public string? Unit {get;set;} 
    [Description("Color")]
    public string? Color {get;set;}

    [Description("Price")]
    public decimal? Price { get; set; }

    [Description("RetailPrice")]
    public decimal? RetailPrice { get; set; }


    [Description("Product")]
    public ProductDto Product {get;set;} 

      public string CacheKey => SubProductCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => SubProductCacheKey.Tags;
}
    
    public class CreateSubProductCommandHandler : IRequestHandler<CreateSubProductCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateSubProductCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(CreateSubProductCommand request, CancellationToken cancellationToken)
        {
           var item = SubProductMapper.FromCreateCommand(request);
           // raise a create domain event
	       //item.AddDomainEvent(new SubProductCreatedEvent(item));
           _context.SubProducts.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

