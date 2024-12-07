// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Products.DTOs;

[Description("Products")]
public record ProductDto
{

    [Description("Id")] public int Id { get; set; }

    [Description("Product Name")] public string? Name { get; set; }

    [Description("Product Code")] public string? Code { get; set; }

    [Description("Description")] public string? Description { get; set; }

    [Description("Unit")] public string? Unit { get; set; }

    [Description("Brand Name")] public string? Brand { get; set; }

    [Description("Price")] public decimal Price { get; set; }

    [Description("Pictures")] public List<ProductImage>? Pictures { get; set; }

    [Description("RetailPrice")] public decimal? RetailPrice { get; set; }

    [Description("Stock")]  public int? Stock { get; set; }
}