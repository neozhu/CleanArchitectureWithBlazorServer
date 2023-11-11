// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Products.DTOs;

[Description("Products")]
public class ProductDto
{
    [Description("Id")] public int Id { get; set; }

    [Description("Product Name")] public string? Name { get; set; }

    [Description("Description")] public string? Description { get; set; }

    [Description("Unit")] public string? Unit { get; set; }

    [Description("Brand Name")] public string? Brand { get; set; }

    [Description("Price")] public decimal Price { get; set; }

    [Description("Pictures")] public List<ProductImage>? Pictures { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}