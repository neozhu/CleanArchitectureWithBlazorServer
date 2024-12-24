// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using System;

namespace CleanArchitecture.Blazor.Domain.Entities;


public class Invoice : BaseAuditableEntity
{
    public int? OfferId { get; set; }
    public int? SupplierId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; } = 0m;
    public string Status { get; set; } = "Pending";
    public List<InvoiceLine> InvoiceLines { get; set; } = [];

    public virtual Offer? Offer { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
