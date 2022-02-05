// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Products.Queries.Pagination;

    public class ProductsWithPaginationQuery : PaginationRequest, IRequest<PaginatedData<ProductDto>>
    {
       
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
            //TODO:Implementing ProductsWithPaginationQueryHandler method 
           var filters = PredicateBuilder.FromFilter<Product>(request.FilterRules);
           var data = await _context.Products.Where(filters)
                .OrderBy("{request.Sort} {request.Order}")
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.Page, request.Rows);
            return data;
        }
   }