using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Specifications;
using System.Diagnostics;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Queries.GetById;

public class GetInvoiceLineByIdQuery : ICacheableRequest<Result<InvoiceLineDto>>
{
   public required int Id { get; set; }
   public string CacheKey => InvoiceLineCacheKey.GetByIdCacheKey($"{Id}");
   public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
}

public class GetInvoiceLineByIdQueryHandler :
     IRequestHandler<GetInvoiceLineByIdQuery, Result<InvoiceLineDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInvoiceLineByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InvoiceLineDto>> Handle(GetInvoiceLineByIdQuery request, CancellationToken cancellationToken)
    {
        //var data = await _context.InvoiceLines.ApplySpecification(new InvoiceLineByIdSpecification(request.Id))
        //                                        .ProjectTo()
        //                                        .FirstAsync(cancellationToken);
        //return await Result<InvoiceLineDto>.SuccessAsync(data);

        return null;
    }
}
