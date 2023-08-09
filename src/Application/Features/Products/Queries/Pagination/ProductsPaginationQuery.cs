// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;

public class ProductsWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<ProductDto>>
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public string? Keyword { get; set; }
    public ProductListView ListView { get; set; } = ProductListView.All; //<-- When the user selects a different ListView,
    public UserProfile? CurrentUser { get; set; } // <-- This CurrentUser property gets its value from the information of

    public string CacheKey => ProductCacheKey.GetPaginationCacheKey($"{this}");

    public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;

    // the currently logged in user
    public override string ToString()
    {
        return
            $"CurrentUser:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},Name:{Name},Brand:{Brand},Unit:{Unit},MinPrice:{MinPrice},MaxPrice:{MaxPrice},SortDirection:{SortDirection},OrderBy:{OrderBy},{ PageNumber},{PageSize}";
    }

    public SearchProductSpecification Specification => new SearchProductSpecification(this);
}

public class ProductsWithPaginationQueryHandler :
    IRequestHandler<ProductsWithPaginationQuery, PaginatedData<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<ProductsWithPaginationQueryHandler> _localizer;
    private readonly IMapper _mapper;

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

    public async Task<PaginatedData<ProductDto>> Handle(ProductsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {

        var data = await _context.Products.OrderBy($"{request.OrderBy} {request.SortDirection}")
                        .ProjectToPaginatedDataAsync<Product,ProductDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}