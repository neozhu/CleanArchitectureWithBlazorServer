

//using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
//using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;
//using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
//using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Specifications;

//namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Queries.Export;

//public class ExportInvoiceLinesQuery : InvoiceLineAdvancedFilter, ICacheableRequest<Result<byte[]>>
//{
//      public InvoiceLineAdvancedSpecification Specification => new InvoiceLineAdvancedSpecification(this);
//      public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
//    public override string ToString()
//    {
//        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}";
//    }
//    public string CacheKey => InvoiceLineCacheKey.GetExportCacheKey($"{this}");
//}
    
//public class ExportInvoiceLinesQueryHandler :
//         IRequestHandler<ExportInvoiceLinesQuery, Result<byte[]>>
//{
//        private readonly IApplicationDbContext _context;
//        private readonly IExcelService _excelService;
//        private readonly IStringLocalizer<ExportInvoiceLinesQueryHandler> _localizer;
//        private readonly InvoiceLineDto _dto = new();
//        public ExportInvoiceLinesQueryHandler(
//            IApplicationDbContext context,
//            IExcelService excelService,
//            IStringLocalizer<ExportInvoiceLinesQueryHandler> localizer
//            )
//        {
//            _context = context;
//            _excelService = excelService;
//            _localizer = localizer;
//        }
//        #nullable disable warnings
//        public async Task<Result<byte[]>> Handle(ExportInvoiceLinesQuery request, CancellationToken cancellationToken)
//        {
//            var data = await _context.InvoiceLines.ApplySpecification(request.Specification)
//                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
//                       .ProjectTo()
//                       .AsNoTracking()
//                       .ToListAsync(cancellationToken);
//            var result = await _excelService.ExportAsync(data,
//                new Dictionary<string, Func<InvoiceLineDto, object?>>()
//                {
//                    // TODO: Define the fields that should be exported, for example:
//                    {_localizer[_dto.GetMemberDescription(x=>x.InvoiceId)],item => item.InvoiceId}, 
//{_localizer[_dto.GetMemberDescription(x=>x.ProductId)],item => item.ProductId}, 
//{_localizer[_dto.GetMemberDescription(x=>x.Quantity)],item => item.Quantity}, 
//{_localizer[_dto.GetMemberDescription(x=>x.UnitPrice)],item => item.UnitPrice}, 
//{_localizer[_dto.GetMemberDescription(x=>x.LineTotal)],item => item.LineTotal}, 
//{_localizer[_dto.GetMemberDescription(x=>x.Discount)],item => item.Discount}, 

//                }
//                , _localizer[_dto.GetClassDescription()]);
//            return await Result<byte[]>.SuccessAsync(result);
//        }
//}
