
// Usage:
// The `OfferLineDto` class is used to represent offerline data throughout the CleanArchitecture.Blazor
// application, providing a well-defined contract for passing offerline information between different 
// layers and services. Each property includes a description for better understandability during 
// serialization and documentation generation.

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;

[Description("OfferLines")]
public record OfferLineDto
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


}

