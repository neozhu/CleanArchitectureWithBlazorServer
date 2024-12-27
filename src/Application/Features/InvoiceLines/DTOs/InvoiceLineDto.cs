
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;

[Description("InvoiceLines")]
public record InvoiceLineDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("Invoice id")]
    public int InvoiceId {get;set;} 
    [Description("Product id")]
    public int ProductId {get;set;} 
    [Description("Quantity")]
    public int Quantity {get;set;} 
    [Description("Unit price")]
    public decimal UnitPrice {get;set;} 
    [Description("Line total")]
    public decimal LineTotal {get;set;} 
    [Description("Discount")]
    public decimal Discount {get;set;} 
    [Description("Product")]
    public ProductDto Product {get;set;}  = new ProductDto();


}

