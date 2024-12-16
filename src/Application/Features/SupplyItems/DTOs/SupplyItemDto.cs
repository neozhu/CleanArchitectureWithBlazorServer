
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.DTOs;

[Description("SupplyItems")]
public class SupplyItemDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("Product id")]
    public int ProductId {get;set;} 
    [Description("Supplier id")]
    public int SupplierId {get;set;} 
    [Description("Quantity")]
    public int Quantity {get;set;} 
    [Description("Cost per item")]
    public decimal CostPerItem {get;set;} 
    [Description("Notes")]
    public string? Notes {get;set;} 
    [Description("Product")]
    public ProductDto Product {get;set;} 
    [Description("Supplier")]
    public SupplierDto Supplier {get;set;} 


}

