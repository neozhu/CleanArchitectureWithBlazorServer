using CleanArchitecture.Blazor.Application.Invoices.Mappers;
using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
using CleanArchitecture.Blazor.Application.Features.Invoices.DTOs;
using CleanArchitecture.Blazor.Application.Features.Invoices.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Queries.Export;

public class ExportInvoicesQuery : InvoiceAdvancedFilter, ICacheableRequest<Result<byte[]>>
{
    public InvoiceAdvancedSpecification Specification => new InvoiceAdvancedSpecification(this);
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}";
    }
    public string CacheKey => InvoiceCacheKey.GetExportCacheKey($"{this}");
}

public class ExportInvoicesQueryHandler :
         IRequestHandler<ExportInvoicesQuery, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportInvoicesQueryHandler> _localizer;
    private readonly InvoiceDto _dto = new();
    public ExportInvoicesQueryHandler(
        IApplicationDbContext context,
        IExcelService excelService,
        IStringLocalizer<ExportInvoicesQueryHandler> localizer
        )
    {
        _context = context;
        _excelService = excelService;
        _localizer = localizer;
    }
#nullable disable warnings
    public async Task<Result<byte[]>> Handle(ExportInvoicesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Invoices.ApplySpecification(request.Specification)
                   .OrderBy($"{request.OrderBy} {request.SortDirection}")
                   .ProjectTo()
                   .AsNoTracking()
                   .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<InvoiceDto, object?>>()
            {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.OfferId)],item => item.OfferId},
{_localizer[_dto.GetMemberDescription(x=>x.InvoiceDate)],item => item.InvoiceDate},
{_localizer[_dto.GetMemberDescription(x=>x.TotalAmount)],item => item.TotalAmount},
{_localizer[_dto.GetMemberDescription(x=>x.Status)],item => item.Status},

            }
            , _localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}
