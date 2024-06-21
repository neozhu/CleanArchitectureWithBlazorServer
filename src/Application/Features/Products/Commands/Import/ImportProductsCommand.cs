// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Import;

public class ImportProductsCommand : ICacheInvalidatorRequest<Result<int>>
{
    public ImportProductsCommand(string fileName, byte[] data)
    {
        FileName = fileName;
        Data = data;
    }

    public string FileName { get; }
    public byte[] Data { get; }
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => ProductCacheKey.GetOrCreateTokenSource();
}

public record CreateProductsTemplateCommand : IRequest<Result<byte[]>>
{
}

public class ImportProductsCommandHandler :
    IRequestHandler<CreateProductsTemplateCommand, Result<byte[]>>,
    IRequestHandler<ImportProductsCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ImportProductsCommandHandler> _localizer;
    private readonly IMapper _mapper;
    private readonly ISerializer _serializer;

    public ImportProductsCommandHandler(
        IApplicationDbContext context,
        IExcelService excelService,
        ISerializer serializer,
        IStringLocalizer<ImportProductsCommandHandler> localizer,
        IMapper mapper
    )
    {
        _context = context;
        _localizer = localizer;
        _excelService = excelService;
        _serializer = serializer;
        _mapper = mapper;
    }

    public async Task<Result<byte[]>> Handle(CreateProductsTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[]
        {
            _localizer["Brand Name"],
            _localizer["Product Name"],
            _localizer["Description"],
            _localizer["Unit"],
            _localizer["Price of unit"],
            _localizer["Pictures"]
        };
        var result = await _excelService.CreateTemplateAsync(fields, _localizer["Products"]);
        return await Result<byte[]>.SuccessAsync(result);
    }
#nullable disable warnings
    public async Task<Result<int>> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        var result = await _excelService.ImportAsync(request.Data,
            new Dictionary<string, Func<DataRow, ProductDto, object?>>
            {
                { _localizer["Brand Name"], (row, item) => item.Brand = row[_localizer["Brand Name"]].ToString() },
                { _localizer["Product Name"], (row, item) => item.Name = row[_localizer["Product Name"]].ToString() },
                {
                    _localizer["Description"],
                    (row, item) => item.Description = row[_localizer["Description"]].ToString()
                },
                { _localizer["Unit"], (row, item) => item.Unit = row[_localizer["Unit"]].ToString() },
                {
                    _localizer["Price of unit"],
                    (row, item) => item.Price = row.FieldDecimalOrDefault(_localizer["Price of unit"])
                },
                {
                    _localizer["Pictures"],
                    (row, item) => item.Pictures = string.IsNullOrEmpty(row[_localizer["Pictures"]].ToString())
                        ? new List<ProductImage>()
                        : _serializer.Deserialize<List<ProductImage>>(row[_localizer["Pictures"]].ToString())
                }
            }, _localizer["Products"]);
        if (!result.Succeeded) return await Result<int>.FailureAsync(result.Errors);
        {
            foreach (var dto in result.Data!)
            {
                var item = _mapper.Map<Product>(dto);
                await _context.Products.AddAsync(item, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result.Data.Count());
        }
    }
}