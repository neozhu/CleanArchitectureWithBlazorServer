// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.AnalyzeAccountSecurity;

public class AnalyzeAccountSecurityQuery : ICacheableRequest<Result<SecurityAnalysisDto>>
{
    public string UserId { get; set; } = string.Empty;
    public int AnalysisPeriodDays { get; set; } = 30;
    public bool IncludeFailedLogins { get; set; } = true;

    public string CacheKey => LoginAuditCacheKey.GetPaginationCacheKey($"SecurityAnalysis_{UserId}_{AnalysisPeriodDays}_{IncludeFailedLogins}");
    public IEnumerable<string>? Tags => LoginAuditCacheKey.Tags;

    public override string ToString()
    {
        return $"UserId:{UserId},AnalysisPeriodDays:{AnalysisPeriodDays},IncludeFailedLogins:{IncludeFailedLogins}";
    }
}

public class AnalyzeAccountSecurityQueryHandler : IRequestHandler<AnalyzeAccountSecurityQuery, Result<SecurityAnalysisDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ILogger<AnalyzeAccountSecurityQueryHandler> _logger;

    public AnalyzeAccountSecurityQueryHandler(
        IApplicationDbContext context,
        IIdentityService identityService,
        ILogger<AnalyzeAccountSecurityQueryHandler> logger)
    {
        _context = context;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result<SecurityAnalysisDto>> Handle(AnalyzeAccountSecurityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-request.AnalysisPeriodDays);
            
            // Get user information
            var userName = await _identityService.GetUserNameAsync(request.UserId, cancellationToken);
            if (string.IsNullOrEmpty(userName))
            {
                return await Result<SecurityAnalysisDto>.FailureAsync("User not found");
            }

            // Get login history
            var loginHistory = await _context.LoginAudits
                .Where(l => l.UserId == request.UserId && l.LoginTimeUtc >= startDate)
                .OrderBy(l => l.LoginTimeUtc)
                .ToListAsync(cancellationToken);

            if (!loginHistory.Any())
            {
                return await Result<SecurityAnalysisDto>.SuccessAsync(new SecurityAnalysisDto
                {
                    UserId = request.UserId,
                    UserName = userName,
                    AnalysisPeriodDays = request.AnalysisPeriodDays,
                    OverallRiskLevel = SecurityRiskLevel.Low,
                    RiskScore = 0,
                    Recommendations = new List<string> { "No recent login activity found" }
                });
            }

            // Perform security analysis
            var securityThreats = new List<SecurityThreatDto>();
            
            // Analyze new IP addresses
            await AnalyzeNewIpAddresses(request.UserId, loginHistory, securityThreats, cancellationToken);
            
            // Analyze suspicious locations
            await AnalyzeSuspiciousLocations(loginHistory, securityThreats);
            
            // Analyze failed login patterns
            if (request.IncludeFailedLogins)
            {
                await AnalyzeFailedLoginPatterns(loginHistory, securityThreats);
            }
            
            // Analyze unusual login times
            await AnalyzeUnusualLoginTimes(loginHistory, securityThreats);
            
            // Analyze new devices
            await AnalyzeNewDevices(request.UserId, loginHistory, securityThreats, cancellationToken);
            
            // Analyze concurrent sessions
            await AnalyzeConcurrentSessions(loginHistory, securityThreats);
            
            // Analyze rapid geographic movement
            await AnalyzeRapidGeographicMovement(loginHistory, securityThreats);

            // Calculate overall risk score and level
            var (riskScore, riskLevel) = CalculateOverallRisk(securityThreats);
            
            // Generate recommendations
            var recommendations = GenerateRecommendations(securityThreats, riskLevel);

            var result = new SecurityAnalysisDto
            {
                UserId = request.UserId,
                UserName = userName,
                AnalysisPeriodDays = request.AnalysisPeriodDays,
                OverallRiskLevel = riskLevel,
                RiskScore = riskScore,
                SecurityThreats = securityThreats,
                Recommendations = recommendations,
                ShouldChangePassword = riskLevel >= SecurityRiskLevel.Medium
            };

            return await Result<SecurityAnalysisDto>.SuccessAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing account security for user {UserId}", request.UserId);
            return await Result<SecurityAnalysisDto>.FailureAsync("Error occurred during security analysis");
        }
    }

    private async Task AnalyzeNewIpAddresses(string userId, List<LoginAudit> recentLogins, List<SecurityThreatDto> threats, CancellationToken cancellationToken)
    {
        var historicalStartDate = DateTime.UtcNow.AddDays(-90);
        var historicalIps = await _context.LoginAudits
            .Where(l => l.UserId == userId && l.LoginTimeUtc < recentLogins.First().LoginTimeUtc && l.LoginTimeUtc >= historicalStartDate)
            .Select(l => l.IpAddress)
            .Distinct()
            .ToListAsync(cancellationToken);

        var recentIps = recentLogins.Select(l => l.IpAddress).Distinct().ToList();
        var newIps = recentIps.Where(ip => !string.IsNullOrEmpty(ip) && !historicalIps.Contains(ip)).ToList();

        if (newIps.Any())
        {
            var firstNewIpLogin = recentLogins.Where(l => newIps.Contains(l.IpAddress)).MinBy(l => l.LoginTimeUtc);
            var lastNewIpLogin = recentLogins.Where(l => newIps.Contains(l.IpAddress)).MaxBy(l => l.LoginTimeUtc);

            threats.Add(new SecurityThreatDto
            {
                ThreatType = SecurityThreatType.NewIpAddress,
                RiskLevel = newIps.Count > 3 ? SecurityRiskLevel.High : SecurityRiskLevel.Medium,
                Description = $"Login from {newIps.Count} new IP address(es)",
                Details = $"New IP addresses: {string.Join(", ", newIps)}",
                FirstDetected = firstNewIpLogin?.LoginTimeUtc ?? DateTime.UtcNow,
                LastDetected = lastNewIpLogin?.LoginTimeUtc ?? DateTime.UtcNow,
                OccurrenceCount = newIps.Count,
                AdditionalData = new Dictionary<string, object>
                {
                    ["NewIpAddresses"] = newIps
                }
            });
        }
    }

    private Task AnalyzeSuspiciousLocations(List<LoginAudit> loginHistory, List<SecurityThreatDto> threats)
    {
        var locations = loginHistory
            .Where(l => !string.IsNullOrEmpty(l.Region))
            .GroupBy(l => l.Region)
            .Select(g => new { Region = g.Key, Count = g.Count(), FirstSeen = g.Min(x => x.LoginTimeUtc) })
            .ToList();

        var suspiciousLocations = locations.Where(l => l.Count == 1).ToList();

        if (suspiciousLocations.Any())
        {
            threats.Add(new SecurityThreatDto
            {
                ThreatType = SecurityThreatType.SuspiciousLocation,
                RiskLevel = suspiciousLocations.Count > 2 ? SecurityRiskLevel.High : SecurityRiskLevel.Medium,
                Description = $"Login from {suspiciousLocations.Count} unusual location(s)",
                Details = $"Locations: {string.Join(", ", suspiciousLocations.Select(l => l.Region))}",
                FirstDetected = suspiciousLocations.Min(l => l.FirstSeen),
                LastDetected = suspiciousLocations.Max(l => l.FirstSeen),
                OccurrenceCount = suspiciousLocations.Count,
                AdditionalData = new Dictionary<string, object>
                {
                    ["SuspiciousLocations"] = suspiciousLocations.Select(l => l.Region).ToList()
                }
            });
        }
        
        return Task.CompletedTask;
    }

    private Task AnalyzeFailedLoginPatterns(List<LoginAudit> loginHistory, List<SecurityThreatDto> threats)
    {
        var failedLogins = loginHistory.Where(l => !l.Success).ToList();
        
        if (failedLogins.Count >= 5)
        {
            var riskLevel = failedLogins.Count >= 10 ? SecurityRiskLevel.Critical : SecurityRiskLevel.High;
            
            threats.Add(new SecurityThreatDto
            {
                ThreatType = SecurityThreatType.MultipleFailedLogins,
                RiskLevel = riskLevel,
                Description = $"{failedLogins.Count} failed login attempts",
                Details = $"Failed login attempts from {failedLogins.Select(f => f.IpAddress).Distinct().Count()} different IP addresses",
                FirstDetected = failedLogins.Min(f => f.LoginTimeUtc),
                LastDetected = failedLogins.Max(f => f.LoginTimeUtc),
                OccurrenceCount = failedLogins.Count,
                AdditionalData = new Dictionary<string, object>
                {
                    ["FailedLoginIps"] = failedLogins.Select(f => f.IpAddress).Distinct().ToList(),
                    ["FailedLoginCount"] = failedLogins.Count
                }
            });
        }
        
        return Task.CompletedTask;
    }

    private Task AnalyzeUnusualLoginTimes(List<LoginAudit> loginHistory, List<SecurityThreatDto> threats)
    {
        var unusualTimeLogins = loginHistory
            .Where(l => l.LoginTimeUtc.Hour < 6 || l.LoginTimeUtc.Hour > 22)
            .ToList();

        if (unusualTimeLogins.Count >= 3)
        {
            threats.Add(new SecurityThreatDto
            {
                ThreatType = SecurityThreatType.UnusualLoginTime,
                RiskLevel = SecurityRiskLevel.Medium,
                Description = $"{unusualTimeLogins.Count} logins during unusual hours",
                Details = "Logins detected during late night or early morning hours (10 PM - 6 AM UTC)",
                FirstDetected = unusualTimeLogins.Min(l => l.LoginTimeUtc),
                LastDetected = unusualTimeLogins.Max(l => l.LoginTimeUtc),
                OccurrenceCount = unusualTimeLogins.Count,
                AdditionalData = new Dictionary<string, object>
                {
                    ["UnusualLoginTimes"] = unusualTimeLogins.Select(l => l.LoginTimeUtc).ToList()
                }
            });
        }
        
        return Task.CompletedTask;
    }

    private async Task AnalyzeNewDevices(string userId, List<LoginAudit> recentLogins, List<SecurityThreatDto> threats, CancellationToken cancellationToken)
    {
        var historicalStartDate = DateTime.UtcNow.AddDays(-90);
        var historicalBrowsers = await _context.LoginAudits
            .Where(l => l.UserId == userId && l.LoginTimeUtc < recentLogins.First().LoginTimeUtc && l.LoginTimeUtc >= historicalStartDate)
            .Select(l => l.BrowserInfo)
            .Distinct()
            .ToListAsync(cancellationToken);

        var recentBrowsers = recentLogins.Select(l => l.BrowserInfo).Distinct().ToList();
        var newBrowsers = recentBrowsers.Where(b => !string.IsNullOrEmpty(b) && !historicalBrowsers.Contains(b)).ToList();

        if (newBrowsers.Any())
        {
            var firstNewDeviceLogin = recentLogins.Where(l => newBrowsers.Contains(l.BrowserInfo)).MinBy(l => l.LoginTimeUtc);
            var lastNewDeviceLogin = recentLogins.Where(l => newBrowsers.Contains(l.BrowserInfo)).MaxBy(l => l.LoginTimeUtc);

            threats.Add(new SecurityThreatDto
            {
                ThreatType = SecurityThreatType.NewDevice,
                RiskLevel = newBrowsers.Count > 2 ? SecurityRiskLevel.High : SecurityRiskLevel.Medium,
                Description = $"Login from {newBrowsers.Count} new device(s)/browser(s)",
                Details = $"New devices detected",
                FirstDetected = firstNewDeviceLogin?.LoginTimeUtc ?? DateTime.UtcNow,
                LastDetected = lastNewDeviceLogin?.LoginTimeUtc ?? DateTime.UtcNow,
                OccurrenceCount = newBrowsers.Count,
                AdditionalData = new Dictionary<string, object>
                {
                    ["NewBrowsers"] = newBrowsers
                }
            });
        }
    }

    private Task AnalyzeConcurrentSessions(List<LoginAudit> loginHistory, List<SecurityThreatDto> threats)
    {
        var successfulLogins = loginHistory.Where(l => l.Success).OrderBy(l => l.LoginTimeUtc).ToList();
        var concurrentSessions = new List<(DateTime time, int sessionCount, List<string> locations)>();

        for (int i = 0; i < successfulLogins.Count; i++)
        {
            var currentLogin = successfulLogins[i];
            var timeWindow = TimeSpan.FromMinutes(30);
            
            var simultaneousLogins = successfulLogins
                .Where(l => Math.Abs((l.LoginTimeUtc - currentLogin.LoginTimeUtc).TotalMinutes) <= timeWindow.TotalMinutes)
                .ToList();

            if (simultaneousLogins.Count >= 2)
            {
                var locations = simultaneousLogins.Select(l => l.Region).Where(r => !string.IsNullOrEmpty(r)).Distinct().Cast<string>().ToList();
                if (locations.Count >= 2)
                {
                    concurrentSessions.Add((currentLogin.LoginTimeUtc, simultaneousLogins.Count, locations));
                }
            }
        }

        if (concurrentSessions.Any())
        {
            threats.Add(new SecurityThreatDto
            {
                ThreatType = SecurityThreatType.ConcurrentSessions,
                RiskLevel = SecurityRiskLevel.High,
                Description = $"Concurrent login sessions detected",
                Details = $"Multiple simultaneous logins from different locations",
                FirstDetected = concurrentSessions.Min(c => c.time),
                LastDetected = concurrentSessions.Max(c => c.time),
                OccurrenceCount = concurrentSessions.Count,
                AdditionalData = new Dictionary<string, object>
                {
                    ["ConcurrentSessions"] = concurrentSessions
                }
            });
        }
        
        return Task.CompletedTask;
    }

    private Task AnalyzeRapidGeographicMovement(List<LoginAudit> loginHistory, List<SecurityThreatDto> threats)
    {
        var successfulLogins = loginHistory
            .Where(l => l.Success && !string.IsNullOrEmpty(l.Region))
            .OrderBy(l => l.LoginTimeUtc)
            .ToList();

        var rapidMovements = new List<(DateTime firstLogin, DateTime secondLogin, string fromLocation, string toLocation, double hoursBetween)>();

        for (int i = 0; i < successfulLogins.Count - 1; i++)
        {
            var currentLogin = successfulLogins[i];
            var nextLogin = successfulLogins[i + 1];

            if (currentLogin.Region != nextLogin.Region)
            {
                var timeBetween = nextLogin.LoginTimeUtc - currentLogin.LoginTimeUtc;
                
                // If login time between different regions is less than 4 hours, it may be suspicious
                if (timeBetween.TotalHours < 4)
                {
                    rapidMovements.Add((currentLogin.LoginTimeUtc, nextLogin.LoginTimeUtc, 
                                      currentLogin.Region!, nextLogin.Region!, timeBetween.TotalHours));
                }
            }
        }

        if (rapidMovements.Any())
        {
            threats.Add(new SecurityThreatDto
            {
                ThreatType = SecurityThreatType.RapidGeographicMovement,
                RiskLevel = SecurityRiskLevel.High,
                Description = "Rapid geographic movement detected",
                Details = $"Login location changes that may be physically impossible",
                FirstDetected = rapidMovements.Min(r => r.firstLogin),
                LastDetected = rapidMovements.Max(r => r.secondLogin),
                OccurrenceCount = rapidMovements.Count,
                AdditionalData = new Dictionary<string, object>
                {
                    ["RapidMovements"] = rapidMovements
                }
            });
        }
        
        return Task.CompletedTask;
    }

    private (int riskScore, SecurityRiskLevel riskLevel) CalculateOverallRisk(List<SecurityThreatDto> threats)
    {
        if (!threats.Any())
            return (0, SecurityRiskLevel.Low);

        var riskScore = threats.Sum(t => (int)t.RiskLevel * 10);
        
        var riskLevel = riskScore switch
        {
            >= 40 => SecurityRiskLevel.Critical,
            >= 25 => SecurityRiskLevel.High,
            >= 10 => SecurityRiskLevel.Medium,
            _ => SecurityRiskLevel.Low
        };

        return (riskScore, riskLevel);
    }

    private List<string> GenerateRecommendations(List<SecurityThreatDto> threats, SecurityRiskLevel overallRisk)
    {
        var recommendations = new List<string>();

        if (overallRisk >= SecurityRiskLevel.Medium)
        {
            recommendations.Add("Change your password immediately");
            recommendations.Add("Enable two-factor authentication if not already enabled");
        }

        if (threats.Any(t => t.ThreatType == SecurityThreatType.NewIpAddress))
        {
            recommendations.Add("Review recent login locations and verify all activities are authorized");
        }

        if (threats.Any(t => t.ThreatType == SecurityThreatType.MultipleFailedLogins))
        {
            recommendations.Add("Monitor for potential brute force attacks and consider temporary IP blocking");
        }

        if (threats.Any(t => t.ThreatType == SecurityThreatType.ConcurrentSessions))
        {
            recommendations.Add("Log out from all devices and log back in from trusted devices only");
        }

        if (threats.Any(t => t.ThreatType == SecurityThreatType.RapidGeographicMovement))
        {
            recommendations.Add("Verify all recent login activities and report any unauthorized access");
        }

        if (overallRisk == SecurityRiskLevel.Critical)
        {
            recommendations.Add("Contact system administrator immediately for account security review");
            recommendations.Add("Consider temporarily disabling the account until security review is complete");
        }

        if (!recommendations.Any())
        {
            recommendations.Add("Continue following security best practices");
            recommendations.Add("Regularly update your password and review login activities");
        }

        return recommendations;
    }
} 