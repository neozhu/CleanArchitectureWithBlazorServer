// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Export;

public class ExportProductsQuery : ProductAdvancedFilter, IRequest<Result<byte[]>>
{
    public ExportType ExportType { get; set; }
    public ProductAdvancedSpecification Specification => new(this);
}

public class ExportProductsQueryHandler :
    IRequestHandler<ExportProductsQuery, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportProductsQueryHandler> _localizer;
    private readonly IMapper _mapper;
    private readonly IPDFService _pdfService;
    private readonly ISerializer _serializer;

    public ExportProductsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ISerializer serializer,
        IExcelService excelService,
        IPDFService pdfService,
        IStringLocalizer<ExportProductsQueryHandler> localizer
    )
    {
        _context = context;
        _mapper = mapper;
        _serializer = serializer;
        _excelService = excelService;
        _pdfService = pdfService;
        _localizer = localizer;
    }
#nullable disable warnings
    public async Task<Result<byte[]>> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products.ApplySpecification(request.Specification)
            .AsNoTracking()
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);


        byte[] result;
        Dictionary<string, Func<ProductDto, object?>> mappers;
        switch (request.ExportType)
        {
            case ExportType.PDF:
                mappers = new Dictionary<string, Func<ProductDto, object?>>
                {
                    { _localizer["Brand Name"], item => item.Brand },
                    { _localizer["Product Name"], item => item.Name },
                    { _localizer["Description"], item => item.Description },
                    { _localizer["Price of unit"], item => item.Price },
                    { _localizer["Unit"], item => item.Unit }
                    //{ _localizer["Pictures"], item => string.Join(",",item.Pictures??new string[]{ }) },
                };
                result = await _pdfService.ExportAsync(data, mappers, _localizer["Products"], true);
                break;
            default:
                mappers = new Dictionary<string, Func<ProductDto, object?>>
                {
                    { _localizer["Brand Name"], item => item.Brand },
                    { _localizer["Product Name"], item => item.Name },
                    { _localizer["Description"], item => item.Description },
                    { _localizer["Price of unit"], item => item.Price },
                    { _localizer["Unit"], item => item.Unit },
                    { _localizer["Pictures"], item => _serializer.Serialize(item.Pictures) }
                };
                result = await _excelService.ExportAsync(data, mappers, _localizer["Products"]);
                break;
        }

        return await Result<byte[]>.SuccessAsync(result);
    }
}