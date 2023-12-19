// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll;

public class GetAllProductsQuery : ICacheableRequest<IEnumerable<ProductDto>>
{
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;
}

public class GetProductQuery : ICacheableRequest<ProductDto>
{
    public required int Id { get; set; }

    public string CacheKey => ProductCacheKey.GetProductByIdCacheKey(Id);
    public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;
}

public class GetAllProductsQueryHandler :
    IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>,
    IRequestHandler<GetProductQuery, ProductDto>

{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<GetAllProductsQueryHandler> _localizer;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllProductsQueryHandler> localizer
    )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        //TODO:Implementing GetAllProductsQueryHandler method 
        var data = await _context.Products
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }

    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products.Where(x => x.Id == request.Id)
                       .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                       .FirstOrDefaultAsync(cancellationToken) ??
                   throw new NotFoundException($"Product with id: {request.Id} not found.");
        return data;
    }
}