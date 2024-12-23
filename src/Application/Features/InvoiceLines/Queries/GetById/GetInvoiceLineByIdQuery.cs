using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Queries.GetById;

public class GetInvoiceLineByIdQuery : ICacheableRequest<Result<InvoiceLineDto>>
{
   public required int Id { get; set; }
   public required int InvoiceId { get; set; }
   public string CacheKey => InvoiceLineCacheKey.GetByIdCacheKey($"{Id}");
   public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
}

public class GetInvoiceLineByIdQueryHandler(
    IApplicationDbContext context) :
     IRequestHandler<GetInvoiceLineByIdQuery, Result<InvoiceLineDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<InvoiceLineDto>> Handle(GetInvoiceLineByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Invoices
              .Where(o => o.Id == request.InvoiceId)
              .SelectMany(o => o.InvoiceLines)
              .Where(ol => ol.Id == request.Id)
              .AsNoTracking()
              .ProjectTo()
              .FirstOrDefaultAsync(cancellationToken);

        return await Result<InvoiceLineDto>.SuccessAsync(data);
    }
}
