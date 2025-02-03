
using CleanArchitecture.Blazor.Application.Features.Offers.Caching;
using CleanArchitecture.Blazor.Application.Features.Offers.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Offers.Commands.Create;

public class CreateOfferCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Customer id")]
    public int CustomerId {get;set;} 
    [Description("Offer date")]
    public DateTime OfferDate {get;set;} 
    [Description("Total amount")]
    public decimal TotalAmount {get;set;}

    [Description("ShippingCosts")]
    public decimal ShippingCosts { get; set; }

    [Description("Status")]
    public string? Status {get;set;}

    [Description("Packaging")]
    public int? Packaging { get; set; }

    [Description("Draft")]
    public int? Draft { get; set; }

    [Description("StatDesignus")]
    public string? Design { get; set; }

    [Description("ShippingMethod")]
    public string? ShippingMethod { get; set; }

    [Description("AdvancePaymentAmount")]
    public decimal AdvancePaymentAmount { get; set; } = 0m;
    //[Description("Offer lines")]
    //public List<OfferLineDtoDto>? OfferLines {get;set;} 

    public string CacheKey => OfferCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => OfferCacheKey.Tags;
}
    
    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateOfferCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
           var item = OfferMapper.FromCreateCommand(request);
           // raise a create domain event
	       //item.AddDomainEvent(new OfferCreatedEvent(item));
           _context.Offers.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

