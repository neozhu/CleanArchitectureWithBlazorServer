// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;

public class ProductsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<ProductDto>>, ICacheable
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public override string ToString()
    {
        return $"{base.ToString()},Name:{Name},Brand:{Brand},Unit:{Unit},MinPrice:{MinPrice},MaxPrice:{MaxPrice}";
    }
    public string CacheKey => ProductCacheKey.GetPagtionCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;
}

public class ProductsWithPaginationQueryHandler :
         IRequestHandler<ProductsWithPaginationQuery, PaginatedData<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<ProductsWithPaginationQueryHandler> _localizer;

    public ProductsWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<ProductsWithPaginationQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<PaginatedData<ProductDto>> Handle(ProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products.Specify(new SearchProductSpecification(request))
             .OrderBy($"{request.OrderBy} {request.SortDirection}")
             .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
             .PaginatedDataAsync(request.PageNumber, request.PageSize);
        return data;
    }
}