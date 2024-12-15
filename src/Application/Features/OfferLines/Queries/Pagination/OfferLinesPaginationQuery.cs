

using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Specifications;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Queries.Pagination;

public class OfferLinesWithPaginationQuery : OfferLineAdvancedFilter, ICacheableRequest<PaginatedData<OfferLineDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => OfferLineCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
    public OfferLineAdvancedSpecification Specification => new(this);

    public int OrderId { get; set; }
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
            var offerLines = await _context.Offers
                                       .Where(x => x.Id == request.OrderId)
                                       .SelectMany(x => x.OfferLines)
                                       .Include(x=>x.Product)
                                       .AsNoTracking()
                                       .ProjectToPaginatedDataAsync(request.PageNumber,
                                                                              request.PageSize,
                                                                              OfferLineMapper.ToDto,
                                                                              cancellationToken);
            return offerLines;

    }
}