

using CleanArchitecture.Blazor.Application.Features.Suppliers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Suppliers.Caching;
using CleanArchitecture.Blazor.Application.Features.Suppliers.Mappers;
using CleanArchitecture.Blazor.Application.Features.Suppliers.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Suppliers.Queries.Pagination;

public class SuppliersWithPaginationQuery : SupplierAdvancedFilter, ICacheableRequest<PaginatedData<SupplierDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => SupplierCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => SupplierCacheKey.Tags;
    public SupplierAdvancedSpecification Specification => new SupplierAdvancedSpecification(this);
}
    
public class SuppliersWithPaginationQueryHandler :
         IRequestHandler<SuppliersWithPaginationQuery, PaginatedData<SupplierDto>>
{
        private readonly IApplicationDbContext _context;

        public SuppliersWithPaginationQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedData<SupplierDto>> Handle(SuppliersWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Suppliers.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                   .ProjectToPaginatedDataAsync(request.Specification, 
                                                                                request.PageNumber, 
                                                                                request.PageSize, 
                                                                                SupplierMapper.ToDto, 
                                                                                cancellationToken);
            return data;
        }
}