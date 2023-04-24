// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;

public class CustomersWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<CustomerDto>>
{
    [CompareTo("Name", "Description")] // <-- This filter will be applied to Name or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    [CompareTo(typeof(SearchCustomersWithListView), "Id")]
    public CustomerListView ListView { get; set; } = CustomerListView.All; //<-- When the user selects a different ListView,
                                                                               // a custom query expression is executed on the filter.
    public override string ToString()
    {
        return $"Listview:{ListView},Search:{Keyword},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }
    [IgnoreFilter]
    public string CacheKey => CustomerCacheKey.GetPaginationCacheKey($"{this}");
    [IgnoreFilter]
    public MemoryCacheEntryOptions? Options => CustomerCacheKey.MemoryCacheEntryOptions;
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
           var data = await _context.Customers.ApplyFilterWithoutPagination(request)
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.Page, request.PerPage);
            return data;
        }
}

public class CustomersPaginationSpecification : Specification<Customer>
{
    public CustomersPaginationSpecification(CustomersWithPaginationQuery query)
    {
        Criteria = q => q.Name != null;
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            And(x => x.Name.Contains(query.Keyword));
        }
       
    }
}
public class SearchCustomersWithListView : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd",CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd",CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        var end30 = Convert.ToDateTime(today.AddDays(30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        var listview = (CustomerListView)value;
        return listview switch {
            CustomerListView.All => expressionBody,
            CustomerListView.CreatedToday => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                                          Expression.Constant(start, typeof(DateTime?)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                     Expression.Constant(end, typeof(DateTime?))), 
                                                     CombineType.And),
            CustomerListView.Created30Days => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                                          Expression.Constant(start, typeof(DateTime?)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                     Expression.Constant(end30, typeof(DateTime?))), 
                                                     CombineType.And),
            _=> expressionBody
        };
    }
}
public enum CustomerListView
{
    [Description("All")]
    All,
    [Description("Created Toady")]
    CreatedToday,
    [Description("Created within the last 30 days")]
    Created30Days
}