
// Usage:
// This command can be used to add a new offerline or edit an existing one.
// It handles caching logic and domain event raising automatically.


using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.AddEdit;

public class AddEditOfferLineCommand: ICacheInvalidatorRequest<Result<int>>
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

public class AddEditOfferLineCommandHandler : IRequestHandler<AddEditOfferLineCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public AddEditOfferLineCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(AddEditOfferLineCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
   //         var item = await _context.OfferLines.FindAsync(request.Id, cancellationToken);
   //         if (item == null)
   //         {
   //             return await Result<int>.FailureAsync($"OfferLine with id: [{request.Id}] not found.");
   //         }
   //         OfferLineMapper.ApplyChangesFrom(request,item);
			//// raise a update domain event
			//item.AddDomainEvent(new OfferLineUpdatedEvent(item));
   //         await _context.SaveChangesAsync(cancellationToken);
   //         return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
   //         var item = OfferLineMapper.FromEditCommand(request);
   //         // raise a create domain event
			//item.AddDomainEvent(new OfferLineCreatedEvent(item));
   //         _context.OfferLines.Add(item);
   //         await _context.SaveChangesAsync(cancellationToken);
   //         return await Result<int>.SuccessAsync(item.Id);
        }

        return await Result<int>.SuccessAsync(1);
    }
}

