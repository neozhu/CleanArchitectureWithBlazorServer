
using CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Suppliers.Mappers;
using CleanArchitecture.Blazor.Application.Features.Suppliers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Suppliers.Queries.GetAll;

public class GetAllSuppliersQuery : ICacheableRequest<IEnumerable<SupplierDto>>
{
   public string CacheKey => SupplierCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => SupplierCacheKey.Tags;
}

public class GetAllSuppliersQueryHandler :
     IRequestHandler<GetAllSuppliersQuery, IEnumerable<SupplierDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSuppliersQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Suppliers.ProjectTo()
                                                .AsNoTracking()
                                                .ToListAsync(cancellationToken);
        return data;
    }
}


