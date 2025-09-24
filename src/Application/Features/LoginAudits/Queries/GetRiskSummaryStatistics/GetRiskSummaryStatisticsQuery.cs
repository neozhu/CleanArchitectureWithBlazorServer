// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;
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

    private readonly IApplicationDbContextFactory _dbContextFactory;

    public GetRiskSummaryStatisticsQueryHandler(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<RiskSummaryStatisticsDto> Handle(GetRiskSummaryStatisticsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var summaries = await db.UserLoginRiskSummaries
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
            LastAnalysisTime = summaries.Any() ? summaries.Max(x => x.LastModifiedAt ?? x.CreatedAt) : null
        };

        return statistics;
    }
}
