// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;

    public class ProductsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<ProductDto>>
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
           var data = await _context.Products.Where(x=>x.Name.Contains(request.Keyword) || x.Description.Contains(request.Keyword))
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
   }