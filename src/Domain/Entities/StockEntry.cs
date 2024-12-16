// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using System;

namespace CleanArchitecture.Blazor.Domain.Entities;


public class StockEntry : BaseAuditableEntity
{
    public int ProductId { get; set; } // Foreign Key
    public string ChangeType { get; set; } // "IN" or "OUT"
    public int Quantity { get; set; }
    public string Note { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public Product Product { get; set; }
}