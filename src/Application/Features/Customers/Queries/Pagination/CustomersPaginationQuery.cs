// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;

public class CustomersWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<CustomerDto>>
{
    public CustomerListView ListView { get; set; } = CustomerListView.All; 
    public UserProfile? CurrentUser { get; set; }
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => CustomerCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => CustomerCacheKey.MemoryCacheEntryOptions;
    public CustomersPaginationSpecification Specification => new CustomersPaginationSpecification(this);
}
    
public class CustomersWithPaginationQueryHandler :
         IRequestHandler<CustomersWithPaginationQuery, PaginatedData<CustomerDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CustomersWithPaginationQueryHandler> _localizer;

        public CustomersWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<CustomersWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<CustomerDto>> Handle(CustomersWithPaginationQuery request, CancellationToken cancellationToken)
        {
           // TODO: Implement CustomersWithPaginationQueryHandler method 
           var data = await _context.Customers.Specify(request.Specification)
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
}

public class CustomersPaginationSpecification : Specification<Customer>
{
    public CustomersPaginationSpecification(CustomersWithPaginationQuery query)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);

       Query.Where(q => q.Name != null)
             .Where(q => q.Name!.Contains(query.Keyword) || q.Description!.Contains(query.Keyword), !string.IsNullOrEmpty(query.Keyword))
             .Where(q => q.CreatedBy == query.CurrentUser.UserId, query.ListView == CustomerListView.My && query.CurrentUser is not null)
             .Where(q => q.Created >= start && q.Created <= end, query.ListView == CustomerListView.CreatedToday)
             .Where(q => q.Created >= last30day, query.ListView == CustomerListView.Created30Days);
       
    }
}

public enum CustomerListView
{
    [Description("All")]
    All,
    [Description("My")]
    My,
    [Description("Created Toady")]
    CreatedToday,
    [Description("Created within the last 30 days")]
    Created30Days
}