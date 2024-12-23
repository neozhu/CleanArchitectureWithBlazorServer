// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using System;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class InvoiceLine : BaseAuditableEntity
{
    public int InvoiceId { get; set; } // Foreign Key
    public int ProductId { get; set; } // Foreign Key
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public decimal Discount { get; set; } = 0m;
    public Product Product { get; set; } = null!;
}
