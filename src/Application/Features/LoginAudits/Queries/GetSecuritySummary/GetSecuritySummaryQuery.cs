// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetSecuritySummary;

public class GetSecuritySummaryQuery : ICacheableRequest<Result<SecuritySummaryDto>>
{
    public string UserId { get; set; } = string.Empty;

    public string CacheKey => LoginAuditCacheKey.GetPaginationCacheKey($"SecuritySummary_{UserId}");
    public IEnumerable<string>? Tags => LoginAuditCacheKey.Tags;

    public override string ToString()
    {
        return $"UserId:{UserId}";
    }
}

public class GetSecuritySummaryQueryHandler : IRequestHandler<GetSecuritySummaryQuery, Result<SecuritySummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetSecuritySummaryQueryHandler> _logger;

    public GetSecuritySummaryQueryHandler(
        IApplicationDbContext context,
        ILogger<GetSecuritySummaryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<SecuritySummaryDto>> Handle(GetSecuritySummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            var last7Days = DateTime.UtcNow.AddDays(-7);

            // 获取最近的登录数据
            var recentLogins = await _context.LoginAudits
                .Where(l => l.UserId == request.UserId && l.LoginTimeUtc >= last30Days)
                .OrderByDescending(l => l.LoginTimeUtc)
                .ToListAsync(cancellationToken);

            if (!recentLogins.Any())
            {
                return await Result<SecuritySummaryDto>.SuccessAsync(new SecuritySummaryDto
                {
                    UserId = request.UserId,
                    RiskLevel = SecurityRiskLevel.Low,
                    HasSecurityWarnings = false,
                    LastLoginDate = null,
                    NewDevicesCount = 0,
                    NewIpAddressesCount = 0,
                    FailedLoginsLast7Days = 0,
                    ShouldChangePassword = false
                });
            }

            // 快速计算关键安全指标
            var last7DaysLogins = recentLogins.Where(l => l.LoginTimeUtc >= last7Days).ToList();
            var failedLoginsLast7Days = last7DaysLogins.Count(l => !l.Success);
            
            // 获取历史数据进行比较
            var historical = await _context.LoginAudits
                .Where(l => l.UserId == request.UserId && l.LoginTimeUtc < last30Days && l.LoginTimeUtc >= DateTime.UtcNow.AddDays(-90))
                .ToListAsync(cancellationToken);

            var historicalIps = historical.Select(l => l.IpAddress).Distinct().ToList();
            var historicalBrowsers = historical.Select(l => l.BrowserInfo).Distinct().ToList();

            var recentIps = recentLogins.Select(l => l.IpAddress).Distinct().ToList();
            var recentBrowsers = recentLogins.Select(l => l.BrowserInfo).Distinct().ToList();

            var newIpAddresses = recentIps.Where(ip => !string.IsNullOrEmpty(ip) && !historicalIps.Contains(ip)).Count();
            var newDevices = recentBrowsers.Where(b => !string.IsNullOrEmpty(b) && !historicalBrowsers.Contains(b)).Count();

            // 简单的风险评估
            var riskLevel = SecurityRiskLevel.Low;
            var hasWarnings = false;

            if (failedLoginsLast7Days >= 5 || newIpAddresses >= 3 || newDevices >= 2)
            {
                riskLevel = SecurityRiskLevel.High;
                hasWarnings = true;
            }
            else if (failedLoginsLast7Days >= 2 || newIpAddresses >= 1 || newDevices >= 1)
            {
                riskLevel = SecurityRiskLevel.Medium;
                hasWarnings = true;
            }

            var result = new SecuritySummaryDto
            {
                UserId = request.UserId,
                RiskLevel = riskLevel,
                HasSecurityWarnings = hasWarnings,
                LastLoginDate = recentLogins.FirstOrDefault()?.LoginTimeUtc,
                NewDevicesCount = newDevices,
                NewIpAddressesCount = newIpAddresses,
                FailedLoginsLast7Days = failedLoginsLast7Days,
                ShouldChangePassword = riskLevel >= SecurityRiskLevel.Medium
            };

            return await Result<SecuritySummaryDto>.SuccessAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting security summary for user {UserId}", request.UserId);
            return await Result<SecuritySummaryDto>.FailureAsync("Error occurred while retrieving security summary");
        }
    }
} 