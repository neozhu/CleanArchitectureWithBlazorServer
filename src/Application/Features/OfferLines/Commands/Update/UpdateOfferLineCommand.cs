using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Update;

public class UpdateOfferLineCommand: ICacheInvalidatorRequest<Result<int>>
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

public class UpdateOfferLineCommandHandler : IRequestHandler<UpdateOfferLineCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public UpdateOfferLineCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(UpdateOfferLineCommand request, CancellationToken cancellationToken)
    {

        //   var item = await _context.OfferLines.FindAsync(request.Id, cancellationToken);
        //   if (item == null)
        //   {
        //       return await Result<int>.FailureAsync($"OfferLine with id: [{request.Id}] not found.");
        //   }
        //   OfferLineMapper.ApplyChangesFrom(request, item);
        // // raise a update domain event
        //item.AddDomainEvent(new OfferLineUpdatedEvent(item));
        //   await _context.SaveChangesAsync(cancellationToken);
        //   return await Result<int>.SuccessAsync(item.Id);

        return await Result<int>.SuccessAsync(0);
    }
}

