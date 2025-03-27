﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : ICacheInvalidatorRequest<Result>
{
    public DeleteProductCommand(int[] id)
    {
        Id = id;
    }

    public int[] Id { get; }
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class DeleteProductCommandHandler :
    IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Products.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.AddDomainEvent(new DeletedEvent<Product>(item));
            _context.Products.Remove(item);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}