

using CleanArchitecture.Blazor.Application.Features.Offers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Offers.Caching;
using CleanArchitecture.Blazor.Application.Features.Offers.Mappers;
using CleanArchitecture.Blazor.Application.Features.Offers.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Offers.Queries.Pagination;

public class OffersWithPaginationQuery : OfferAdvancedFilter, ICacheableRequest<PaginatedData<OfferDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => OfferCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => OfferCacheKey.Tags;
    public OfferAdvancedSpecification Specification => new(this);
}
    
public class OffersWithPaginationQueryHandler :
         IRequestHandler<OffersWithPaginationQuery, PaginatedData<OfferDto>>
{
        private readonly IApplicationDbContext _context;

        public OffersWithPaginationQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedData<OfferDto>> Handle(OffersWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Offers
            .Include(x=>x.Customer)
            .Include(x => x.OfferLines)
            .ThenInclude(x=>x.Product)
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                   .ProjectToPaginatedDataAsync(request.Specification, 
                                                                                request.PageNumber, 
                                                                                request.PageSize, 
                                                                                OfferMapper.ToDto, 
                                                                                cancellationToken);
            return data;
        }
}