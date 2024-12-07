
using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Queries.Export;

public class ExportOfferLinesQuery : OfferLineAdvancedFilter, ICacheableRequest<Result<byte[]>>
{
      public OfferLineAdvancedSpecification Specification => new OfferLineAdvancedSpecification(this);
      public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}";
    }
    public string CacheKey => OfferLineCacheKey.GetExportCacheKey($"{this}");
}
    
public class ExportOfferLinesQueryHandler :
         IRequestHandler<ExportOfferLinesQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportOfferLinesQueryHandler> _localizer;
        private readonly OfferLineDto _dto = new();
        public ExportOfferLinesQueryHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ExportOfferLinesQueryHandler> localizer
            )
        {
            _context = context;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportOfferLinesQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.OfferLines.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo()
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<OfferLineDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.OfferId)],item => item.OfferId}, 
{_localizer[_dto.GetMemberDescription(x=>x.ItemId)],item => item.ItemId}, 
{_localizer[_dto.GetMemberDescription(x=>x.Quantity)],item => item.Quantity}, 
{_localizer[_dto.GetMemberDescription(x=>x.Discount)],item => item.Discount}, 
{_localizer[_dto.GetMemberDescription(x=>x.LineTotal)],item => item.LineTotal}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
}
