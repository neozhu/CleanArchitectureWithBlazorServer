// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.Specifications;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetMyLoginHistory;

public class GetMyLoginHistoryQuery : ICacheableRequest<PaginatedData<LoginAuditDto>>
{
    public string UserId { get; set; } = string.Empty;
    public string? Keyword { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string OrderBy { get; set; } = "LoginTimeUtc";
    public string SortDirection { get; set; } = "desc";

    public MyLoginHistorySpecification Specification => new(this);

    public string CacheKey => LoginAuditCacheKey.GetPaginationCacheKey($"My_{UserId}_{Keyword}_{PageNumber}_{PageSize}_{OrderBy}_{SortDirection}");
    public IEnumerable<string>? Tags => LoginAuditCacheKey.Tags;

    public override string ToString()
    {
        return $"UserId:{UserId},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},PageNumber:{PageNumber},PageSize:{PageSize}";
    }
}

public class GetMyLoginHistoryQueryHandler : IRequestHandler<GetMyLoginHistoryQuery, PaginatedData<LoginAuditDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetMyLoginHistoryQueryHandler(
        IMapper mapper,
        IApplicationDbContext context
    )
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<PaginatedData<LoginAuditDto>> Handle(GetMyLoginHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.LoginAudits.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<LoginAudit, LoginAuditDto>(request.Specification, request.PageNumber, request.PageSize,
                _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}
