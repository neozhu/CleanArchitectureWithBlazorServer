// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Razor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Products.Queries.Export;

    public class ExportProductsQuery : IRequest<byte[]>
    {
        public string FilterRules { get; set; }
        public string Sort { get; set; } = "Id";
        public string Order { get; set; } = "desc";
    }
    
    public class ExportProductsQueryHandler :
         IRequestHandler<ExportProductsQuery, byte[]>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportProductsQueryHandler> _localizer;

        public ExportProductsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportProductsQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task<byte[]> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
        {
            //TODO:Implementing ExportProductsQueryHandler method 
            var filters = PredicateBuilder.FromFilter<Product>(request.FilterRules);
            var data = await _context.Products.Where(filters)
                       .OrderBy("{request.Sort} {request.Order}")
                       .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<ProductDto, object>>()
                {
                    //{ _localizer["Id"], item => item.Id },
                }
                , _localizer["Products"]);
            return result;
        }
    }