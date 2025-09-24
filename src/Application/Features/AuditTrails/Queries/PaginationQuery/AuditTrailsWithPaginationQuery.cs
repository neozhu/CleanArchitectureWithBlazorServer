// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;

public class AuditTrailsWithPaginationQuery : AuditTrailAdvancedFilter, ICacheableRequest<PaginatedData<AuditTrailDto>>
{
    public AuditTrailAdvancedSpecification Specification => new(this);
    public string CacheKey => AuditTrailsCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => AuditTrailsCacheKey.Tags;

    public override string ToString()
    {
        return
            $"Listview:{ListView}-{CurrentUser?.UserId},AuditType:{AuditType},Search:{Keyword},Sort:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}

public class AuditTrailsQueryHandler : IRequestHandler<AuditTrailsWithPaginationQuery, PaginatedData<AuditTrailDto>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public AuditTrailsQueryHandler(
        IApplicationDbContextFactory dbContextFactory,
        IMapper mapper
    )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<PaginatedData<AuditTrailDto>> Handle(AuditTrailsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.AuditTrails.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<AuditTrail, AuditTrailDto>(request.Specification, request.PageNumber,
                request.PageSize, _mapper.ConfigurationProvider, cancellationToken);

        return data;
    }
}
