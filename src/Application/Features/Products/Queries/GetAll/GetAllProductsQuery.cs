// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll;

public class GetAllProductsQuery : ICacheableRequest<IEnumerable<ProductDto>>
{
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class GetProductQuery : ICacheableRequest<ProductDto?>
{
    public required int Id { get; set; }

    public string CacheKey => ProductCacheKey.GetProductByIdCacheKey(Id);
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class GetAllProductsQueryHandler :
    IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>,
    IRequestHandler<GetProductQuery, ProductDto?>

{
    private readonly IApplicationDbContext _context;

    public GetAllProductsQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
            .ProjectTo()
            .ToListAsync(cancellationToken);
        return data;
    }

    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products.Where(x => x.Id == request.Id)
                       .ProjectTo()
                       .FirstOrDefaultAsync(cancellationToken);
        return data;
    }
}