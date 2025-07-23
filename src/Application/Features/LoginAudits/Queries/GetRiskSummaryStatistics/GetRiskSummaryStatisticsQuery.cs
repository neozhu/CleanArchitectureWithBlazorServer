// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetRiskSummaryStatistics;

public class GetRiskSummaryStatisticsQuery : ICacheableRequest<RiskSummaryStatisticsDto>
{
    public string CacheKey => "RiskSummaryStatistics";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(15);
    public IEnumerable<string>? Tags => new[] { "userloginrisksummary", "statistics" };
}

public class GetRiskSummaryStatisticsQueryHandler : IRequestHandler<GetRiskSummaryStatisticsQuery, RiskSummaryStatisticsDto>
{
    private readonly IApplicationDbContext _context;

    public GetRiskSummaryStatisticsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RiskSummaryStatisticsDto> Handle(GetRiskSummaryStatisticsQuery request, CancellationToken cancellationToken)
    {
        var summaries = await _context.UserLoginRiskSummaries
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var statistics = new RiskSummaryStatisticsDto
        {
            TotalUsers = summaries.Count,
            LowRiskUsers = summaries.Count(x => x.RiskLevel == SecurityRiskLevel.Low),
            MediumRiskUsers = summaries.Count(x => x.RiskLevel == SecurityRiskLevel.Medium),
            HighRiskUsers = summaries.Count(x => x.RiskLevel == SecurityRiskLevel.High),
            CriticalRiskUsers = summaries.Count(x => x.RiskLevel == SecurityRiskLevel.Critical),
            AverageRiskScore = summaries.Any() ? summaries.Average(x => x.RiskScore) : 0,
            TotalRiskAnalyses = summaries.Count,
            LastAnalysisTime = summaries.Any() ? summaries.Max(x => x.LastModified ?? x.Created) : null
        };

        return statistics;
    }
}
