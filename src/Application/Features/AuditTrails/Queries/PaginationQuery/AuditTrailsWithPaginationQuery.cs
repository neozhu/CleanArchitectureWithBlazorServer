// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;

public class AuditTrailsWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<AuditTrailDto>>
{
    [CompareTo("TableName", "UserId")] // <-- This filter will be applied to Name or Brand or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    [OperatorComparison(OperatorType.Equal)]
    public AuditType? AuditType { get; set; }
    [CompareTo(typeof(SearchAuditTrailsWithListView),"Id")]
    public AuditTrailListView ListView { get; set; }= AuditTrailListView.All;
    public override string ToString()
    {
        return $"Listview:{ListView},AuditType:{AuditType},Search:{Keyword},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }
    public string CacheKey => AuditTrailsCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => AuditTrailsCacheKey.MemoryCacheEntryOptions;
}
public class AuditTrailsQueryHandler : IRequestHandler<AuditTrailsWithPaginationQuery, PaginatedData<AuditTrailDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuditTrailsQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _currentUserService = currentUserService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedData<AuditTrailDto>> Handle(AuditTrailsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.AuditTrails.ApplyFilterWithoutPagination(request)
             .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
             .PaginatedDataAsync(request.Page, request.PerPage);

        return data;
    }
    

}
public class SearchAuditTrailsWithListView : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        var last30days= Convert.ToDateTime(today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        //var currentUser = filterProperty.CurrentUser;
        var listview = (AuditTrailListView)value;
        return listview switch
        {
            AuditTrailListView.All => expressionBody,
            AuditTrailListView.Last30days => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "DateTime"),
                                                                          Expression.Constant(last30days, typeof(DateTime)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "DateTime"),
                                                     Expression.Constant(end, typeof(DateTime))),
                                                     CombineType.And),
            AuditTrailListView.CreatedToday => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "DateTime"),
                                                                          Expression.Constant(start, typeof(DateTime)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "DateTime"),
                                                     Expression.Constant(end, typeof(DateTime))),
                                                     CombineType.And),
            _ => expressionBody
        };
    }
}
public enum AuditTrailListView
{
    [Description("All")]
    All,
    [Description("Created Toady")]
    CreatedToday,
    [Description("View of the last 30 days")]
    Last30days,
}