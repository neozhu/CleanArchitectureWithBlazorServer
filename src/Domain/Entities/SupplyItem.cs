
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
    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}