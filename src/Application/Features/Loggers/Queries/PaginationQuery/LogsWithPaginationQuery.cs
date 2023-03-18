// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;
using CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;

namespace CleanArchitecture.Blazor.Application.Logs.Queries.PaginationQuery;

public class LogsWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<LogDto>>
{
    [CompareTo("Message", "Exception", "UserName", "ClientIP")] 
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }

    [CompareTo(typeof(SearchLogsWithListView), "Id")]
    public LogListView ListView { get; set; } = LogListView.All;
    public override string ToString()
    {
        return $"Listview:{ListView},Search:{Keyword},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }
    public string CacheKey => LogsCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => LogsCacheKey.MemoryCacheEntryOptions;
}
public class LogsQueryHandler : IRequestHandler<LogsWithPaginationQuery, PaginatedData<LogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LogsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedData<LogDto>> Handle(LogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Loggers.ApplyFilterWithoutPagination(request)
             .ProjectTo<LogDto>(_mapper.ConfigurationProvider)
             .PaginatedDataAsync(request.Page, request.PerPage);

        return data;
    }
    

}
public class SearchLogsWithListView : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        var last30days = Convert.ToDateTime(today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        var listview = (LogListView)value;
        return listview switch
        {
            LogListView.All => expressionBody,
            LogListView.Information => Expression.Equal(Expression.Property(expressionBody, "Level"),Expression.Constant("Information")),
            LogListView.Warning => Expression.Equal(Expression.Property(expressionBody, "Level"), Expression.Constant("Warning")),
            LogListView.Fatal => Expression.Equal(Expression.Property(expressionBody, "Level"), Expression.Constant("Fatal")),
            LogListView.Error => Expression.Equal(Expression.Property(expressionBody, "Level"), Expression.Constant("Error")),
            LogListView.Last30days => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "TimeStamp"),
                                                                          Expression.Constant(last30days, typeof(DateTime)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "TimeStamp"),
                                                     Expression.Constant(end, typeof(DateTime))),
                                                     CombineType.And),
            LogListView.CreatedToday => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "TimeStamp"),
                                                                          Expression.Constant(start, typeof(DateTime)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "TimeStamp"),
                                                     Expression.Constant(end, typeof(DateTime))),
                                                     CombineType.And),
            _ => expressionBody
        };
    }
}
public enum LogListView
{
    [Description("All")]
    All,
    [Description("Created Toady")]
    CreatedToday,
    [Description("View of the last 30 days")]
    Last30days,
    [Description("View of Information")]
    Information,
    [Description("View of Warning")]
    Warning,
    [Description("View of Error")]
    Error,
    [Description("View of Fatal")]
    Fatal,
}
