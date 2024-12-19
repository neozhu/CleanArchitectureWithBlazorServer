
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Queries.Pagination;

public class InvoiceLinesWithPaginationQuery : InvoiceLineAdvancedFilter, ICacheableRequest<PaginatedData<InvoiceLineDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => InvoiceLineCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
    public InvoiceLineAdvancedSpecification Specification => new InvoiceLineAdvancedSpecification(this);
}
    
public class InvoiceLinesWithPaginationQueryHandler :
         IRequestHandler<InvoiceLinesWithPaginationQuery, PaginatedData<InvoiceLineDto>>
{
        private readonly IApplicationDbContext _context;

        public InvoiceLinesWithPaginationQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedData<InvoiceLineDto>> Handle(InvoiceLinesWithPaginationQuery request, CancellationToken cancellationToken)
        {
        //var data = await _context.InvoiceLines.OrderBy($"{request.OrderBy} {request.SortDirection}")
        //                                        .ProjectToPaginatedDataAsync(request.Specification, 
        //                                                                     request.PageNumber, 
        //                                                                     request.PageSize, 
        //                                                                     InvoiceLineMapper.ToDto, 
        //                                                                     cancellationToken);
        // return data;

        return null;
    }
}