

using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Queries.Pagination;

public class OfferLinesWithPaginationQuery : OfferLineAdvancedFilter, ICacheableRequest<PaginatedData<OfferLineDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => OfferLineCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
    public OfferLineAdvancedSpecification Specification => new OfferLineAdvancedSpecification(this);
}
    
public class OfferLinesWithPaginationQueryHandler :
         IRequestHandler<OfferLinesWithPaginationQuery, PaginatedData<OfferLineDto>>
{
        private readonly IApplicationDbContext _context;

        public OfferLinesWithPaginationQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedData<OfferLineDto>> Handle(OfferLinesWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.OfferLines.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                   .ProjectToPaginatedDataAsync(request.Specification, 
                                                                                request.PageNumber, 
                                                                                request.PageSize, 
                                                                                OfferLineMapper.ToDto, 
                                                                                cancellationToken);
            return data;
        }
}