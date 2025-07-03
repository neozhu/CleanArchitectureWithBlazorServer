// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetUserLoginRiskSummary;

public class GetUserLoginRiskSummaryQuery : ICacheableRequest<UserLoginRiskSummaryDto?>
{
    public required string UserId { get; set; }

    public string CacheKey => $"UserLoginRiskSummary_{UserId}";

    public IEnumerable<string>? Tags => new[] { "userloginrisksummary" };
}

public class GetUserLoginRiskSummaryQueryHandler : IRequestHandler<GetUserLoginRiskSummaryQuery, UserLoginRiskSummaryDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserLoginRiskSummaryQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserLoginRiskSummaryDto?> Handle(GetUserLoginRiskSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _context.UserLoginRiskSummaries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

        return summary != null ? _mapper.Map<UserLoginRiskSummaryDto>(summary) : null;
    }
} 