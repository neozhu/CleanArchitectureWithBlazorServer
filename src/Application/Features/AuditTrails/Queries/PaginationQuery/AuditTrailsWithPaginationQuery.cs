// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Razor.Domain.Entities.Audit;

namespace CleanArchitecture.Razor.Application.AuditTrails.Queries.PaginationQuery;

public class AuditTrailsWithPaginationQuery : PaginationRequest, IRequest<PaginatedData<AuditTrailDto>>
{


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
        var filters = PredicateBuilder.FromFilter<AuditTrail>(request.FilterRules);

        var data = await _context.AuditTrails
            .Where(filters)
            .OrderBy($"{request.Sort} {request.Order}")
            .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.Page, request.Rows);

        return data;
    }


}
