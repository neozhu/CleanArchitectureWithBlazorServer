using CleanArchitecture.Blazor.Application.Invoices.Mappers;
using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
using CleanArchitecture.Blazor.Application.Features.Invoices.DTOs;
using CleanArchitecture.Blazor.Application.Features.Invoices.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Queries.Pagination;

public class InvoicesWithPaginationQuery : InvoiceAdvancedFilter, ICacheableRequest<PaginatedData<InvoiceDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => InvoiceCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;
    public InvoiceAdvancedSpecification Specification => new InvoiceAdvancedSpecification(this);
}

public class InvoicesWithPaginationQueryHandler :
         IRequestHandler<InvoicesWithPaginationQuery, PaginatedData<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;

    public InvoicesWithPaginationQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedData<InvoiceDto>> Handle(InvoicesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _context.Invoices
                .Include(x => x.InvoiceLines)
                .ThenInclude(x => x.Product)
                .AsNoTracking()
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                    .ProjectToPaginatedDataAsync(request.Specification,
                                                                                 request.PageNumber,
                                                                                 request.PageSize,
                                                                                 InvoiceMapper.ToDto,
                                                                                 cancellationToken);
            return data;
        }
        catch (Exception ex)
        {
            var message = $"Error: {ex.Message} - {ex.InnerException?.Message}";
            throw;
        }

    }
}