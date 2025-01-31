

using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.SubProducts.DTOs;

[Description("SubProducts")]
public class SubProductDto
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Prod id")]
    public int ProductId {get;set;} 
    [Description("Unit")]
    public string? Unit {get;set;} 
    [Description("Color")]
    public string? Color {get;set;}

    [Description("SubItemId")]
    public int SubItemId { get; set; }

    [Description("Price")]
    public decimal? Price { get; set; }

    [Description("RetailPrice")]
    public decimal? RetailPrice { get; set; }

    [Description("Product")]
    public ProductDto Product {get;set;} 


}

