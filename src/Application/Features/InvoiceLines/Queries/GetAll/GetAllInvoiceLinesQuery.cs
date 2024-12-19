

using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Queries.GetAll;

public class GetAllInvoiceLinesQuery : ICacheableRequest<IEnumerable<InvoiceLineDto>>
{
   public string CacheKey => InvoiceLineCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
}

public class GetAllInvoiceLinesQueryHandler :
     IRequestHandler<GetAllInvoiceLinesQuery, IEnumerable<InvoiceLineDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllInvoiceLinesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InvoiceLineDto>> Handle(GetAllInvoiceLinesQuery request, CancellationToken cancellationToken)
    {
        //var data = await _context.InvoiceLines.ProjectTo()
        //                                        .AsNoTracking()
        //                                        .ToListAsync(cancellationToken);
        //return data;

        return null;
    }
}


