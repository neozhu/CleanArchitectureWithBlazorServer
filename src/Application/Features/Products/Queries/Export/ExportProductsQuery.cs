// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Mapster;
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
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly TypeAdapterConfig _typeAdapterConfig;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportProductsQueryHandler> _localizer;
    private readonly IPDFService _pdfService;

    public ExportProductsQueryHandler(
        IApplicationDbContextFactory dbContextFactory,
        TypeAdapterConfig typeAdapterConfig,
        IExcelService excelService,
        IPDFService pdfService,
        IStringLocalizer<ExportProductsQueryHandler> localizer
    )
    {
        _dbContextFactory = dbContextFactory;
        _typeAdapterConfig = typeAdapterConfig;
        _excelService = excelService;
        _pdfService = pdfService;
        _localizer = localizer;
    }
#nullable disable warnings
    public async ValueTask<Result<byte[]>> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.Products.ApplySpecification(request.Specification)
            .AsNoTracking()
            .ProjectToType<ProductDto>(_typeAdapterConfig)
            .ToListAsync(cancellationToken);


        byte[] result;
        Dictionary<string, Func<ProductDto, object?>> typeAdapterConfigs;
        switch (request.ExportType)
        {
            case ExportType.PDF:
                typeAdapterConfigs = new Dictionary<string, Func<ProductDto, object?>>
                {
                    { _localizer["Brand Name"], item => item.Brand },
                    { _localizer["Product Name"], item => item.Name },
                    { _localizer["Description"], item => item.Description },
                    { _localizer["Price of unit"], item => item.Price },
                    { _localizer["Unit"], item => item.Unit }
                    //{ _localizer["Pictures"], item => string.Join(",",item.Pictures??new string[]{ }) },
                };
                result = await _pdfService.ExportAsync(data, typeAdapterConfigs, _localizer["Products"], true);
                break;
            default:
                typeAdapterConfigs = new Dictionary<string, Func<ProductDto, object?>>
                {
                    { _localizer["Brand Name"], item => item.Brand },
                    { _localizer["Product Name"], item => item.Name },
                    { _localizer["Description"], item => item.Description },
                    { _localizer["Price of unit"], item => item.Price },
                    { _localizer["Unit"], item => item.Unit },
                    { _localizer["Pictures"], item => JsonSerializer.Serialize(item.Pictures) }
                };
                result = await _excelService.ExportAsync(data, typeAdapterConfigs, _localizer["Products"]);
                break;
        }

        return await Result<byte[]>.SuccessAsync(result);
    }
}
