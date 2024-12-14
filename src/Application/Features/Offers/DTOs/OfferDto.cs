
using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Offers.DTOs;

[Description("Offers")]
public class OfferDto
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Customer id")]
    public int CustomerId { get; set; }
    [Description("Offer date")]
    public DateTime OfferDate { get; set; }
    [Description("Total amount")]
    public decimal TotalAmount { get; set; }
    [Description("Status")]
    public string? Status { get; set; }

    [Description("Offer lines")]
    public List<OfferLine>? OfferLines { get; set; } = [];

    [Description("Offer lines Count")]
    public int OfferLinesCount
    {
        get
        {
            if (OfferLines == null) 
                return 0; 
            
            return OfferLines.Count;
        }
    }

    [Description("Customer")]
    public ContactDto Customer { get; set; }
}

