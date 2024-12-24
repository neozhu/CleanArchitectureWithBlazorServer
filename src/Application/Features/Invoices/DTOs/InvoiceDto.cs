using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.DTOs;

[Description("Invoices")]
public record InvoiceDto
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Offer id")]
    public int? OfferId { get; set; }
    [Description("Invoice date")]
    public DateTime InvoiceDate { get; set; }
    [Description("Total amount")]
    public decimal TotalAmount { get; set; }
    [Description("Status")]
    public string? Status { get; set; }
    [Description("Invoice lines")]
    public List<InvoiceLineDto>? InvoiceLines { get; set; }

    public SupplierDto Supplier { get; set; } = null!;
}

