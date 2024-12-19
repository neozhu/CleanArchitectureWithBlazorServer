using CleanArchitecture.Blazor.Application.Invoices.Mappers;
using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
using CleanArchitecture.Blazor.Application.Features.Invoices.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Import;

public class ImportInvoicesCommand : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; }
    public byte[] Data { get; set; }
    public string CacheKey => InvoiceCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;
    public ImportInvoicesCommand(string fileName, byte[] data)
    {
        FileName = fileName;
        Data = data;
    }
}
public record class CreateInvoicesTemplateCommand : IRequest<Result<byte[]>>
{

}

public class ImportInvoicesCommandHandler :
             IRequestHandler<CreateInvoicesTemplateCommand, Result<byte[]>>,
             IRequestHandler<ImportInvoicesCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<ImportInvoicesCommandHandler> _localizer;
    private readonly IExcelService _excelService;
    private readonly InvoiceDto _dto = new();

    public ImportInvoicesCommandHandler(
        IApplicationDbContext context,
        IExcelService excelService,
        IStringLocalizer<ImportInvoicesCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
        _excelService = excelService;
    }
#nullable disable warnings
    public async Task<Result<int>> Handle(ImportInvoicesCommand request, CancellationToken cancellationToken)
    {

        var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, InvoiceDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.OfferId)], (row, item) => item.OfferId =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.OfferId)]]) },
{ _localizer[_dto.GetMemberDescription(x=>x.InvoiceDate)], (row, item) => item.InvoiceDate =DateTime.Parse(row[_localizer[_dto.GetMemberDescription(x=>x.InvoiceDate)]].ToString()) },
{ _localizer[_dto.GetMemberDescription(x=>x.TotalAmount)], (row, item) => item.TotalAmount =Convert.ToDecimal(row[_localizer[_dto.GetMemberDescription(x=>x.TotalAmount)]]) },
{ _localizer[_dto.GetMemberDescription(x=>x.Status)], (row, item) => item.Status = row[_localizer[_dto.GetMemberDescription(x=>x.Status)]].ToString() },

            }, _localizer[_dto.GetClassDescription()]);
        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await _context.Invoices.AnyAsync(x => x.Id == dto.Id, cancellationToken);
                if (!exists)
                {
                    var item = InvoiceMapper.FromDto(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new InvoiceCreatedEvent(item));
                    await _context.Invoices.AddAsync(item, cancellationToken);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result.Data.Count());
        }
        else
        {
            return await Result<int>.FailureAsync(result.Errors);
        }
    }
    public async Task<Result<byte[]>> Handle(CreateInvoicesTemplateCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement ImportInvoicesCommandHandler method 
        var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.OfferId)],
_localizer[_dto.GetMemberDescription(x=>x.InvoiceDate)],
_localizer[_dto.GetMemberDescription(x=>x.TotalAmount)],
_localizer[_dto.GetMemberDescription(x=>x.Status)],

                };
        var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}

