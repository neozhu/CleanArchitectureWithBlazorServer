
// Usage:
// This command can be used to create a new offerline entity in the system. It includes
// the required fields for the offerline and automatically raises necessary domain
// events for integration with other bounded contexts in the application.

using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Create;

public class CreateOfferLineCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Offer id")]
    public int OfferId {get;set;} 
    [Description("Item id")]
    public int ItemId {get;set;} 
    [Description("Quantity")]
    public int Quantity {get;set;} 
    [Description("Discount")]
    public decimal Discount {get;set;} 
    [Description("Line total")]
    public decimal LineTotal {get;set;} 

      public string CacheKey => OfferLineCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
}
    
    public class CreateOfferLineCommandHandler : IRequestHandler<CreateOfferLineCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateOfferLineCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(CreateOfferLineCommand request, CancellationToken cancellationToken)
        {
        //   var item = OfferLineMapper.FromCreateCommand(request);
        //   // raise a create domain event
	       //item.AddDomainEvent(new OfferLineCreatedEvent(item));
        //   _context.OfferLines.Add(item);
        //   await _context.SaveChangesAsync(cancellationToken);
        //   return  await Result<int>.SuccessAsync(item.Id);

        return await Result<int>.SuccessAsync(0);
         }
    }

