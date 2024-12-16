

using CleanArchitecture.Blazor.Application.Features.SupplyItems.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Queries.Pagination;

public class SupplyItemsWithPaginationQuery : SupplyItemAdvancedFilter, ICacheableRequest<PaginatedData<SupplyItemDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => SupplyItemCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
    public SupplyItemAdvancedSpecification Specification => new SupplyItemAdvancedSpecification(this);
}
    
public class SupplyItemsWithPaginationQueryHandler :
         IRequestHandler<SupplyItemsWithPaginationQuery, PaginatedData<SupplyItemDto>>
{
        private readonly IApplicationDbContext _context;

        public SupplyItemsWithPaginationQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedData<SupplyItemDto>> Handle(SupplyItemsWithPaginationQuery request, CancellationToken cancellationToken)
        {
        //var data = await _context.SupplyItems.OrderBy($"{request.OrderBy} {request.SortDirection}")
        //                                        .ProjectToPaginatedDataAsync(request.Specification, 
        //                                                                     request.PageNumber, 
        //                                                                     request.PageSize, 
        //                                                                     SupplyItemMapper.ToDto, 
        //                                                                     cancellationToken);
        // return data;

        return null;
        }
}