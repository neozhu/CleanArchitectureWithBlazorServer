
using CleanArchitecture.Blazor.Application.Features.SupplyItems.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Queries.Export;

public class ExportSupplyItemsQuery : SupplyItemAdvancedFilter, ICacheableRequest<Result<byte[]>>
{
      public SupplyItemAdvancedSpecification Specification => new SupplyItemAdvancedSpecification(this);
      public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}";
    }
    public string CacheKey => SupplyItemCacheKey.GetExportCacheKey($"{this}");
}
    
public class ExportSupplyItemsQueryHandler :
         IRequestHandler<ExportSupplyItemsQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportSupplyItemsQueryHandler> _localizer;
        private readonly SupplyItemDto _dto = new();
        public ExportSupplyItemsQueryHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ExportSupplyItemsQueryHandler> localizer
            )
        {
            _context = context;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportSupplyItemsQuery request, CancellationToken cancellationToken)
        {
//            var data = await _context.SupplyItems.ApplySpecification(request.Specification)
//                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
//                       .ProjectTo()
//                       .AsNoTracking()
//                       .ToListAsync(cancellationToken);
//            var result = await _excelService.ExportAsync(data,
//                new Dictionary<string, Func<SupplyItemDto, object?>>()
//                {
//                    // TODO: Define the fields that should be exported, for example:
//                    {_localizer[_dto.GetMemberDescription(x=>x.ProductId)],item => item.ProductId}, 
//{_localizer[_dto.GetMemberDescription(x=>x.SupplierId)],item => item.SupplierId}, 
//{_localizer[_dto.GetMemberDescription(x=>x.Quantity)],item => item.Quantity}, 
//{_localizer[_dto.GetMemberDescription(x=>x.CostPerItem)],item => item.CostPerItem}, 
//{_localizer[_dto.GetMemberDescription(x=>x.Notes)],item => item.Notes}, 

//                }
//                , _localizer[_dto.GetClassDescription()]);
//            return await Result<byte[]>.SuccessAsync(result);

            return null;
        }
}
