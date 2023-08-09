// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;

public class AuditTrailsWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<AuditTrailDto>>
{
    public AuditType? AuditType { get; set; }
    public AuditTrailListView ListView { get; set; } = AuditTrailListView.All;
    public string CacheKey => AuditTrailsCacheKey.GetPaginationCacheKey($"{this}");
    public UserProfile? CurrentUser { get; set; }
    public MemoryCacheEntryOptions? Options => AuditTrailsCacheKey.MemoryCacheEntryOptions;
    public AuditTrailsQuerySpec Specification => new AuditTrailsQuerySpec(this);
    public override string ToString()
    {
        return
            $"Listview:{ListView},AuditType:{AuditType},Search:{Keyword},Sort:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}

public class AuditTrailsQueryHandler : IRequestHandler<AuditTrailsWithPaginationQuery, PaginatedData<AuditTrailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
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

    public async Task<PaginatedData<AuditTrailDto>> Handle(AuditTrailsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.AuditTrails.OrderBy($"{request.OrderBy} {request.SortDirection}")
                        .ProjectToPaginatedDataAsync<AuditTrail, AuditTrailDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);

        return data;
    }
}

public enum AuditTrailListView
{
    [Description("All")] All,
    [Description("My Change Histories")] My,
    [Description("Created Toady")] CreatedToday,
    [Description("View of the last 30 days")]
    Last30days
}
public class AuditTrailsQuerySpec : Specification<AuditTrail>
{
    public AuditTrailsQuerySpec(AuditTrailsWithPaginationQuery request)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);

        Query.Where(p => p.AuditType==request.AuditType, request.AuditType is not null) 
             .Where(p => p.UserId == request.CurrentUser.UserId, request.ListView == AuditTrailListView.My && request.CurrentUser is not null)
             .Where(p => p.DateTime.Date == DateTime.Now.Date, request.ListView == AuditTrailListView.CreatedToday)
             .Where(p => p.DateTime >= last30day, request.ListView == AuditTrailListView.Last30days)
             .Where(x => x.TableName.Contains(request.Keyword) , !string.IsNullOrEmpty(request.Keyword));

    }
}