﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Mappers;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public List<ProductImage>? Pictures { get; set; }

    public IReadOnlyList<IBrowserFile>? UploadPictures { get; set; }
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => ProductCacheKey.GetOrCreateTokenSource();
}

public class AddEditProductCommandHandler : IRequestHandler<AddEditProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddEditProductCommandHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddEditProductCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var item = await _context.Products.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (item == null)
            {
                return await Result<int>.FailureAsync($"Prduct with id: [{request.Id}] not found.");
            }
            ProductMapper.MapTo(request, item);
            item.AddDomainEvent(new UpdatedEvent<Product>(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = ProductMapper.Map(request);
            item.AddDomainEvent(new CreatedEvent<Product>(item));
            _context.Products.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}