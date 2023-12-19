// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public List<ProductImage>? Pictures { get; set; }
}

public class ProductImage
{
    public required string Name { get; set; }
    public decimal Size { get; set; }
    public required string Url { get; set; }
}