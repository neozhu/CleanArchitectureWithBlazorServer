



using CleanArchitecture.Blazor.Application.Features.Offers.Caching;
using CleanArchitecture.Blazor.Application.Features.Offers.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Offers.Commands.AddEdit;

public class AddEditOfferCommand: ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
      public int Id { get; set; }
    [Description("Customer id")]
    public int CustomerId {get;set;} 
    [Description("Offer date")]
    public DateTime OfferDate {get;set;}  = DateTime.Now;
    [Description("Total amount")]
    public decimal TotalAmount {get;set;}
    [Description("Status")]
    public string? Status { get; set; } = "Pending";
    [Description("Offer lines")]
    public List<OfferLine>? OfferLines { get; set; }


    [Description("Packaging")]
    public int? Packaging { get; set; }

    [Description("Draft")]
    public int? Draft { get; set; }

    [Description("StatDesignus")]
    public string? Design { get; set; }

    [Description("AdvancePaymentAmount")]
    public decimal AdvancePaymentAmount { get; set; } = 0m;

    [Description("ShippingMethod")]
    public string? ShippingMethod { get; set; }

    [Description("PaymentType")]
    public string? PaymentType { get; set; } = "Cash";

    public string CacheKey => OfferCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => OfferCacheKey.Tags;
}

public class AddEditOfferCommandHandler : IRequestHandler<AddEditOfferCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public AddEditOfferCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(AddEditOfferCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id > 0)
            {
                var item = await _context.Offers.FindAsync(request.Id, cancellationToken);
                if (item == null)
                {
                    return await Result<int>.FailureAsync($"Offer with id: [{request.Id}] not found.");
                }
                OfferMapper.ApplyChangesFrom(request, item);
                // raise a update domain event
                //item.AddDomainEvent(new OfferUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = OfferMapper.FromEditCommand(request);
                // raise a create domain event
                //item.AddDomainEvent(new OfferCreatedEvent(item));
                _context.Offers.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
        }
        catch (Exception e)
        {
            var ex = e;
            throw;
        }
       
       
    }
}

