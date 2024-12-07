// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Offer : BaseAuditableEntity
{
    public int CustomerId { get; set; }
    public DateTime OfferDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";

    public Contact Customer { get; set; } = null!;
    // Navigation Property
    public List<OfferLine> OfferLines { get; set; } = [];
}
