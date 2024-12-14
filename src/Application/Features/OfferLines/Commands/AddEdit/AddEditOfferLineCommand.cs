

using System.Reactive.Subjects;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.AddEdit;

public class AddEditOfferLineCommand : ICacheInvalidatorRequest<Result<int>>
{

    private decimal _linePrice;
    private int _quantity;
    private decimal _discount = 1; // Represented as a percentage

    [Description("Id")]
    public int Id { get; set; }
    [Description("Offer id")]
    public int OfferId { get; set; }

    private int _itemId;

    [Description("Item id")]
    public int ItemId
    {
        get => _itemId;
        set
        {
            if (_itemId != value)
            {
                _itemId = value;
                ItemIdBehaviorSubject.OnNext(value);  // Notify on change
            }
        }
    }

    [Description("Line Price")]
    public decimal LinePrice
    {
        get => _linePrice;
        set
        {
            _linePrice = value;
            CalculateLineTotal();
        }
    }

    [Description("Quantity")]
    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            CalculateLineTotal();
        }
    }

    [Description("Discount")]
    public decimal Discount
    {
        get => _discount;
        set
        {
            _discount = value;
            CalculateLineTotal();
        }
    }

    [Description("Line total")]
    public decimal LineTotal { get; set; }
    public BehaviorSubject<int> ItemIdBehaviorSubject { get; set; } = new(0);
    public BehaviorSubject<decimal> LineTotalSubject { get; set; } = new(0);
    public string CacheKey => OfferLineCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;

    private void CalculateLineTotal()
    {
        LineTotal = Quantity * _linePrice * (1 - Discount / 100);

        LineTotalSubject.OnNext(LineTotal); // Notify subscribers
    }

    public void Dispose()
    {
        ItemIdBehaviorSubject?.Dispose();
    }
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
        var offer = await _context.Offers.FindAsync(request.OfferId, cancellationToken);

        if (offer is null ) 
            return await Result<int>.FailureAsync($"Offer with with id: [{request.OfferId}] not found.");

        if (request.Id > 0)
        {
            var item = offer.OfferLines.First(x=>x.Id == request.Id);

            if (item == null)
            {
                return await Result<int>.FailureAsync($"OfferLine with id: [{request.Id}] not found.");
            }
            OfferLineMapper.ApplyChangesFrom(request,item);

            //// raise a update domain event
            //item.AddDomainEvent(new OfferLineUpdatedEvent(item));
            await _context.SaveChangesAsync(cancellationToken);
            
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = OfferLineMapper.FromEditCommand(request);
            // raise a create domain event
            //item.AddDomainEvent(new OfferLineCreatedEvent(item));
            //_context.OfferLines.Add(item);
            //await _context.SaveChangesAsync(cancellationToken);
            offer.OfferLines.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            return await Result<int>.SuccessAsync(item.Id);
        }

    }
}

