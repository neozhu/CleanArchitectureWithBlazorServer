// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;

public class AuditTrailsWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<AuditTrailDto>>
{
    [CompareTo("TableName", "UserId")] // <-- This filter will be applied to Name or Brand or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    [OperatorComparison(OperatorType.Equal)]
    public AuditType? AuditType { get; set; }
    public override string ToString()
    {
        return $"Search:{Keyword},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
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
