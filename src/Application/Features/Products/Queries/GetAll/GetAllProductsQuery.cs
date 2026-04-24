// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Mapster;
using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

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
    private readonly TypeAdapterConfig _typeAdapterConfig;
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public GetAllProductsQueryHandler(
        TypeAdapterConfig typeAdapterConfig,
        IApplicationDbContextFactory dbContextFactory
    )
    {
        _typeAdapterConfig = typeAdapterConfig;
        _dbContextFactory = dbContextFactory;
    }

    public async ValueTask<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.Products
            .ProjectToType<ProductDto>(_typeAdapterConfig)
            .ToListAsync(cancellationToken);
        return data;
    }

    public async ValueTask<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.Products.Where(x => x.Id == request.Id)
                       .ProjectToType<ProductDto>(_typeAdapterConfig)
                       .FirstOrDefaultAsync(cancellationToken);
        return data;
    }
}
