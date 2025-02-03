using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Offer : BaseAuditableEntity
{
    public int CustomerId { get; set; }
    public DateTime OfferDate { get; set; } = DateTime.UtcNow;
    public decimal? ShippingCosts { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? PaymentType { get; set; } = "Cash";
    public int? Packaging { get; set; }
    public int? Draft { get;set; }
    public string? Design { get; set; }
    public string? ShippingMethod { get; set; }

    public decimal? AdvancePaymentAmount { get; set; } = 0m;
    public Contact? Customer { get; set; } = null;

    public List<OfferLine> OfferLines { get;set; } = [];
}

