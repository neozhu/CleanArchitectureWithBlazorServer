// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.Products.DTOs;

[Description("Products")]
public class ProductDto
{
    [Display(Name = "Id")] public int Id { get; set; }

    [Display(Name = "Product Name")] public string? Name { get; set; }

    [Display(Name = "Description")] public string? Description { get; set; }

    [Display(Name = "Unit")] public string? Unit { get; set; }

    [Display(Name = "Brand Name")] public string? Brand { get; set; }

    [Display(Name = "Price")] public decimal Price { get; set; }

    [Display(Name = "Pictures")] public List<ProductImage>? Pictures { get; set; }}
