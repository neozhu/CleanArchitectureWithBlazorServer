

using CleanArchitecture.Blazor.Application.Features.Offers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;

[Description("OfferLines")]
public class OfferLineDto
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
    [Description("Offer")]
    public OfferDto Offer {get;set;} 


}

