// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.Products.DTOs;


public class ProductDto:IMapFrom<Product>
    {
 
    public TrackingState TrackingState { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

