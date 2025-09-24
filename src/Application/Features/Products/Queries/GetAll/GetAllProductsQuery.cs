// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    private readonly IMapper _mapper;
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public GetAllProductsQueryHandler(
        IMapper mapper,
        IApplicationDbContextFactory dbContextFactory
    )
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.Products
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }

    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.Products.Where(x => x.Id == request.Id)
                       .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                       .FirstOrDefaultAsync(cancellationToken);
        return data;
    }
}
