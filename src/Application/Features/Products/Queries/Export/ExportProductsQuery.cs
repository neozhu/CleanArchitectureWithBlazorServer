// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Reflection.Metadata;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Export;

public enum ExportType
{
    Excel,
    PDF
}

public class ExportProductsQuery : IRequest<byte[]>
{
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "Desc";
    public string Keyword { get; set; } = String.Empty;
    public ExportType exportType { get; set; }
}

public class ExportProductsQueryHandler :
     IRequestHandler<ExportProductsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IPDFService _pdfService;
    private readonly IStringLocalizer<ExportProductsQueryHandler> _localizer;

    public ExportProductsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IExcelService excelService,
        IPDFService pdfService,
        IStringLocalizer<ExportProductsQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _excelService = excelService;
        _pdfService = pdfService;
        _localizer = localizer;
    }
    public async Task<byte[]> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products.Where(x => x.Name!.Contains(request.Keyword) || x.Description!.Contains(request.Keyword))
                   .OrderBy($"{request.OrderBy} {request.SortDirection}")
                   .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);

        var mappers = new Dictionary<string, Func<ProductDto, object?>>()
                {
                    { _localizer["Brand Name"], item => item.Brand },
                    { _localizer["Product Name"], item => item.Name },
                    { _localizer["Description"], item => item.Description },
                    { _localizer["Price of unit"], item => item.Price },
                    { _localizer["Unit"], item => item.Unit },
                    { _localizer["Pictures"], item => string.Join(",",item.Pictures??new string[]{ }) },
                };

        byte[]? result;
        switch (request.exportType)
        {
            case ExportType.PDF:
                result = await _pdfService.ExportAsync(data, mappers, _localizer["Products"], true);
                break;
            default:
                result = await _excelService.ExportAsync(data, mappers, _localizer["Products"]);
                break;
        }

        return result;
    }
}