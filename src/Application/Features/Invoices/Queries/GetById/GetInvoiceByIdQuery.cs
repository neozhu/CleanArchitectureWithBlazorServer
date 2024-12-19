using CleanArchitecture.Blazor.Application.Invoices.Mappers;
using CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
using CleanArchitecture.Blazor.Application.Features.Invoices.DTOs;
using CleanArchitecture.Blazor.Application.Features.Invoices.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Invoices.Queries.GetById;

public class GetInvoiceByIdQuery : ICacheableRequest<Result<InvoiceDto>>
{
    public required int Id { get; set; }
    public string CacheKey => InvoiceCacheKey.GetByIdCacheKey($"{Id}");
    public IEnumerable<string>? Tags => InvoiceCacheKey.Tags;
}

public class GetInvoiceByIdQueryHandler :
     IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInvoiceByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Invoices.ApplySpecification(new InvoiceByIdSpecification(request.Id))
                                                .ProjectTo()
                                                .FirstAsync(cancellationToken);
        return await Result<InvoiceDto>.SuccessAsync(data);
    }
}
