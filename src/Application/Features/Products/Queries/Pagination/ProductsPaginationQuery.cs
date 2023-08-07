// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;

public class ProductsWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<ProductDto>>
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public Range<decimal> Price { get; set; } = new();
    public string? Keyword { get; set; }
    public ProductListView ListView { get; set; } =
        ProductListView.All; //<-- When the user selects a different ListView,
    public UserProfile? CurrentUser { get; set; } // <-- This CurrentUser property gets its value from the information of

    public string CacheKey => ProductCacheKey.GetPaginationCacheKey($"{this}");

    public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;

    // the currently logged in user
    public override string ToString()
    {
        return
            $"CurrentUser:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},Name:{Name},Brand:{Brand},Unit:{Unit},MinPrice:{Price?.Min},MaxPrice:{Price?.Max},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }

    public Expression<Func<Product, bool>> LinqExpression()
    {
        Expression<Func<Product, bool>> initExpr = product => true;
        Expression<Func<Product, bool>> keywordExpre = product => product.Name.Contains(Keyword) || product.Description.Contains(Keyword);
        Expression<Func<Product, bool>> brandExpre = product => product.Brand.Contains(Brand);
        Expression<Func<Product, bool>> unitExpre = product => product.Unit.Equals(Unit);
        Expression<Func<Product, bool>> priceExpre = product => product.Price >= Price.Min && product.Price <= Price.Max;
        var parameter = Expression.Parameter(typeof(Product));

        switch (ListView)
        {
            case ProductListView.My:
                Expression<Func<Product, bool>> myexp = product => product.CreatedBy == CurrentUser.UserId;
                initExpr = initExpr.And(myexp);
                break;
            case ProductListView.CreatedToday:
                var today = DateTime.Now;
                Expression<Func<Product, bool>> todayexp = product => product.Created.Value.Date == today.Date;
                initExpr = initExpr.And(todayexp);
                break;
            case ProductListView.Created30Days:
                var last30day = DateTime.Now.AddDays(-30);
                Expression<Func<Product, bool>> last30exp = product => product.Created.Value.Date >= last30day.Date;
                initExpr = initExpr.And(last30exp);
                break;
            case ProductListView.All:
            default:
                break;
        }

        if (!string.IsNullOrEmpty(Keyword))
        {
            initExpr = initExpr.And(keywordExpre);
        }
        if (!string.IsNullOrEmpty(Brand))
        {
            initExpr = initExpr.And(brandExpre);
        }
        if (!string.IsNullOrEmpty(Unit))
        {
            initExpr = initExpr.And(unitExpre);
        }
        if (Price != null && Price.Max != null && Price.Min != null)
        {
            initExpr = initExpr.And(priceExpre);
        }
        return initExpr;
    }
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
        var expresss = request.LinqExpression();
        var data = await _context.Products.Where(expresss)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.Page, request.PerPage);
        return data;
    }
}