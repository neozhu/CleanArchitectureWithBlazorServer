
using CleanArchitecture.Blazor.Domain.Common.Entities;
using System;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class SupplyItem : BaseAuditableEntity
{
    public int ProductId { get; set; } // Foreign Key
    public int SupplierId { get; set; } // Foreign Key
    public int Quantity { get; set; }
    public decimal CostPerItem { get; set; }
    public string Notes { get; set; }
    public Product Product { get; set; }
    public Supplier Supplier { get; set; }
}