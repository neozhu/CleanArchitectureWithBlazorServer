// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Import;

public class ImportProductsCommand : IRequest<Result>, ICacheInvalidator
{
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => ProductCacheKey.SharedExpiryTokenSource;

    public string FileName { get;  }
    public byte[] Data { get;  }
    public ImportProductsCommand(string fileName,byte[] data)
    {
        FileName = fileName;
        Data = data;
    }
}
public record CreateProductsTemplateCommand : IRequest<byte[]>
{

}

public class ImportProductsCommandHandler :
             IRequestHandler<CreateProductsTemplateCommand, byte[]>,
             IRequestHandler<ImportProductsCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<ImportProductsCommandHandler> _localizer;
    private readonly IExcelService _excelService;

    public ImportProductsCommandHandler(
        IApplicationDbContext context,
        IExcelService excelService,
        IStringLocalizer<ImportProductsCommandHandler> localizer,
        IMapper mapper
        )
    {
        _context = context;
        _localizer = localizer;
        _excelService = excelService;
        _mapper = mapper;
    }
    public async Task<Result> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {

        var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, ProductDto, object>>
            {
              { _localizer["Brand Name"], (row,item) => item.Brand = row[_localizer["Brand Name"]]?.ToString() },
              { _localizer["Product Name"], (row,item) => item.Name = row[_localizer["Product Name"]]?.ToString() },
              { _localizer["Description"], (row,item) => item.Description = row[_localizer["Description"]]?.ToString() },
              { _localizer["Unit"], (row,item) => item.Unit = row[_localizer["Unit"]]?.ToString() },
              { _localizer["Price of unit"], (row,item) => item.Price =row.IsNull(_localizer["Price of unit"])? 0m:Convert.ToDecimal(row[_localizer["Price of unit"]]) },
              { _localizer["Pictures"], (row,item) => item.Pictures =row.IsNull(_localizer["Pictures"])? null:row[_localizer["Pictures"]].ToString().Split(",").ToList() },
            }, _localizer["Products"]);
        if (result.Succeeded)
        {
            foreach (var dto in result.Data)
            {
                var item = _mapper.Map<Product>(dto);
                await _context.Products.AddAsync(item, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        else
        {
            return Result.Failure(result.Errors);
        }
    }
    public async Task<byte[]> Handle(CreateProductsTemplateCommand request, CancellationToken cancellationToken)
    {
        //TODO:Implementing ImportProductsCommandHandler method 
        var fields = new string[] {
                   //TODO:Defines the title and order of the fields to be imported's template
                   //_localizer["Name"],
                };
        var result = await _excelService.CreateTemplateAsync(fields, _localizer["Products"]);
        return result;
    }
}

