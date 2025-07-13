// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class SecurityAnalysisService : ISecurityAnalysisService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SecurityAnalysisService> _logger;
    private readonly IFusionCache _fusionCache;
    private readonly SecurityAnalysisOptions _options;
    
    // Cache for IP-based analysis to reduce database queries
    private static readonly ConcurrentDictionary<string, DateTime> _lastIpAnalysis = new();

    public SecurityAnalysisService(
        IServiceScopeFactory scopeFactory,
        ILogger<SecurityAnalysisService> logger,
        IFusionCache fusionCache,
        IOptions<SecurityAnalysisOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _fusionCache = fusionCache;
        _options = options.Value;
    }

    public async Task AnalyzeUserSecurityAsync(LoginAudit loginAudit, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var analysisResult = await PerformSecurityAnalysisAsync(dbContext, loginAudit, cancellationToken);
            
            await CreateOrUpdateRiskSummaryAsync(dbContext, loginAudit.UserId, loginAudit.UserName, 
                analysisResult, cancellationToken);

            // Invalidate cache for the user's risk summary
            await _fusionCache.RemoveAsync($"UserLoginRiskSummary_{loginAudit.UserId}", token: cancellationToken);

            _logger.LogInformation("Security analysis completed for user {UserId}. Risk Level: {RiskLevel}, Score: {RiskScore}, Factors: {FactorCount}", 
                loginAudit.UserId, analysisResult.RiskLevel, analysisResult.RiskScore, analysisResult.RiskFactors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze account security for user {UserId}", loginAudit.UserId);
            throw; // Re-throw to ensure calling code is aware of the failure
        }
    }

    private async Task<SecurityAnalysisResult> PerformSecurityAnalysisAsync(
        IApplicationDbContext dbContext, 
        LoginAudit loginAudit, 
        CancellationToken cancellationToken)
    {
        var userId = loginAudit.UserId;
        var currentTime = loginAudit.LoginTimeUtc;
        var historyStartDate = currentTime.AddDays(-_options.HistoryDays);

        // Get historical login data with optimized query
        var userLoginAudits = await GetUserLoginHistoryAsync(dbContext, userId, historyStartDate, cancellationToken);

        var analysisResult = new SecurityAnalysisResult();
        var ruleResults = new List<RiskAnalysisRuleResult>();

        // Execute analysis rules
        ruleResults.Add(await AnalyzeConcentratedFailuresAsync(dbContext, userLoginAudits, loginAudit, cancellationToken));
        ruleResults.Add(AnalyzeNewDeviceOrLocation(userLoginAudits, loginAudit));
        ruleResults.Add(AnalyzeUnusualTimeLogin(loginAudit));

        // Aggregate results
        analysisResult.RiskScore = ruleResults.Sum(r => r.Score);
        analysisResult.RiskFactors = ruleResults.SelectMany(r => r.Factors).ToList();
        analysisResult.RiskScoreBreakdown = ruleResults.ToDictionary(r => r.RuleName, r => r.Score);
        analysisResult.RiskLevel = DetermineRiskLevel(analysisResult.RiskScore);
        
        GenerateSecurityAdvice(analysisResult);

        return analysisResult;
    }

    private async Task<List<LoginAudit>> GetUserLoginHistoryAsync(
        IApplicationDbContext dbContext, 
        string userId, 
        DateTime startDate, 
        CancellationToken cancellationToken)
    {
        return await dbContext.LoginAudits
            .Where(x => x.UserId == userId && x.LoginTimeUtc >= startDate)
            .OrderByDescending(x => x.LoginTimeUtc)
            .Take(1000) // Reasonable limit to prevent excessive memory usage
            .ToListAsync(cancellationToken);
    }

    private async Task<RiskAnalysisRuleResult> AnalyzeConcentratedFailuresAsync(
        IApplicationDbContext dbContext, 
        List<LoginAudit> userLoginAudits, 
        LoginAudit currentLogin, 
        CancellationToken cancellationToken)
    {
        var result = new RiskAnalysisRuleResult { RuleName = "ConcentratedFailures" };
        var bruteForceWindow = currentLogin.LoginTimeUtc.AddMinutes(-_options.BruteForceWindowMinutes);

        // Analyze account-level brute force
        var userFailuresInWindow = userLoginAudits
            .Where(x => !x.Success && 
                       x.LoginTimeUtc >= bruteForceWindow && 
                       x.LoginTimeUtc <= currentLogin.LoginTimeUtc)
            .Count();

        if (userFailuresInWindow >= _options.AccountBruteForceThreshold)
        {
            result.Score += _options.AccountBruteForceScore;
            result.Factors.Add($"Account brute force detected: {userFailuresInWindow} failed login attempts within {_options.BruteForceWindowMinutes} minutes");
        }

        // Analyze IP-level brute force (with caching to reduce database load)
        if (!string.IsNullOrEmpty(currentLogin.IpAddress))
        {
            var cacheKey = $"ip_analysis_{currentLogin.IpAddress}";
            var shouldAnalyzeIp = !_lastIpAnalysis.TryGetValue(cacheKey, out var lastAnalysis) || 
                                 lastAnalysis < DateTime.UtcNow.AddMinutes(-5); // Analyze IP at most every 5 minutes

            if (shouldAnalyzeIp)
            {
                var ipAnalysisResult = await AnalyzeIpBruteForceAsync(dbContext, currentLogin, bruteForceWindow, cancellationToken);
                
                if (ipAnalysisResult.IsTriggered)
                {
                    result.Score += ipAnalysisResult.Score;
                    result.Factors.AddRange(ipAnalysisResult.Factors);
                }

                _lastIpAnalysis.TryAdd(cacheKey, DateTime.UtcNow);
            }
        }

        return result;
    }

    private async Task<RiskAnalysisRuleResult> AnalyzeIpBruteForceAsync(
        IApplicationDbContext dbContext, 
        LoginAudit currentLogin, 
        DateTime bruteForceWindow, 
        CancellationToken cancellationToken)
    {
        var result = new RiskAnalysisRuleResult { RuleName = "IpBruteForce" };

        var ipFailuresQuery = dbContext.LoginAudits
            .Where(x => !x.Success && 
                       x.IpAddress == currentLogin.IpAddress && 
                       x.LoginTimeUtc >= bruteForceWindow && 
                       x.LoginTimeUtc <= currentLogin.LoginTimeUtc);

        var distinctUsersTargeted = await ipFailuresQuery
            .Select(x => x.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalIpFailures = await ipFailuresQuery
            .CountAsync(cancellationToken);

        if (totalIpFailures >= _options.IpBruteForceThreshold && 
            distinctUsersTargeted >= _options.IpBruteForceAccountThreshold)
        {
            result.Score = _options.IpBruteForceScore;
            result.Factors.Add($"IP-based brute force detected: {totalIpFailures} failed attempts from IP {currentLogin.IpAddress} targeting {distinctUsersTargeted} different accounts");
        }

        return result;
    }

    private RiskAnalysisRuleResult AnalyzeNewDeviceOrLocation(List<LoginAudit> userLoginAudits, LoginAudit currentLogin)
    {
        var result = new RiskAnalysisRuleResult { RuleName = "NewDeviceOrLocation" };

        if (!currentLogin.Success) return result;

        var newFactors = new List<string>();

        // Check for new IP address (more efficient with LINQ)
        var hasSeenIpBefore = !string.IsNullOrEmpty(currentLogin.IpAddress) && 
                             userLoginAudits.Any(x => x.Success && 
                                                     x.IpAddress == currentLogin.IpAddress && 
                                                     x.Id != currentLogin.Id);

        // Check for new region
        var hasSeenRegionBefore = !string.IsNullOrEmpty(currentLogin.Region) && 
                                 userLoginAudits.Any(x => x.Success && 
                                                         x.Region == currentLogin.Region && 
                                                         x.Id != currentLogin.Id);

        // Check for new browser info
        var hasSeenBrowserBefore = !string.IsNullOrEmpty(currentLogin.BrowserInfo) && 
                                  userLoginAudits.Any(x => x.Success && 
                                                          x.BrowserInfo == currentLogin.BrowserInfo && 
                                                          x.Id != currentLogin.Id);

        // Evaluate novelty factors
        if (!hasSeenIpBefore && !string.IsNullOrEmpty(currentLogin.IpAddress))
            newFactors.Add($"new IP address ({currentLogin.IpAddress})");
        
        if (!hasSeenRegionBefore && !string.IsNullOrEmpty(currentLogin.Region))
            newFactors.Add($"new geographic location ({currentLogin.Region})");
        
        if (!hasSeenBrowserBefore && !string.IsNullOrEmpty(currentLogin.BrowserInfo))
            newFactors.Add($"new device/browser ({currentLogin.BrowserInfo})");

        if (newFactors.Any())
        {
            result.Score = _options.NewDeviceLocationScore;
            result.Factors.Add($"Login from {string.Join(", ", newFactors)}");
        }

        return result;
    }

    private RiskAnalysisRuleResult AnalyzeUnusualTimeLogin(LoginAudit currentLogin)
    {
        var result = new RiskAnalysisRuleResult { RuleName = "UnusualTimeLogin" };

        if (!currentLogin.Success) return result;

        var loginHour = currentLogin.LoginTimeUtc.Hour;
        
        // Check if login occurred during configured unusual hours
        var isUnusualTime = _options.UnusualTimeStartHour > _options.UnusualTimeEndHour
            ? loginHour >= _options.UnusualTimeStartHour || loginHour < _options.UnusualTimeEndHour
            : loginHour >= _options.UnusualTimeStartHour && loginHour < _options.UnusualTimeEndHour;
        
        if (isUnusualTime)
        {
            result.Score = _options.UnusualTimeScore;
            result.Factors.Add($"Login during unusual hours ({currentLogin.LoginTimeUtc:HH:mm} UTC)");
        }

        return result;
    }

    private SecurityRiskLevel DetermineRiskLevel(int riskScore)
    {
        if (riskScore >= _options.CriticalRiskThreshold)
            return SecurityRiskLevel.Critical;
        if (riskScore >= _options.HighRiskThreshold)
            return SecurityRiskLevel.High;
        if (riskScore >= _options.MediumRiskThreshold)
            return SecurityRiskLevel.Medium;
        
        return SecurityRiskLevel.Low;
    }

    private void GenerateSecurityAdvice(SecurityAnalysisResult analysisResult)
    {
        var riskFactors = analysisResult.RiskFactors;
        var securityAdvice = analysisResult.SecurityAdvice;
        var riskLevel = analysisResult.RiskLevel;

        // Base advice based on risk level
        switch (riskLevel)
        {
            case SecurityRiskLevel.Critical:
                securityAdvice.AddRange(new[]
                {
                    "IMMEDIATE ACTION REQUIRED: Secure your account immediately",
                    "Change your password right now",
                    "Enable two-factor authentication immediately",
                    "Review and revoke access for all devices",
                    "Contact security team if this activity was not authorized"
                });
                break;
                
            case SecurityRiskLevel.High:
                securityAdvice.AddRange(new[]
                {
                    "Immediately review recent account activity",
                    "Consider changing your password",
                    "Enable two-factor authentication if not already active",
                    "Monitor your account closely for the next few days"
                });
                break;
                
            case SecurityRiskLevel.Medium:
                securityAdvice.AddRange(new[]
                {
                    "Monitor your account for unusual activity",
                    "Consider enabling two-factor authentication",
                    "Review your recent login history"
                });
                break;
                
            case SecurityRiskLevel.Low:
                securityAdvice.Add("Your account security appears normal");
                break;
        }

        // Specific advice based on risk factors
        if (riskFactors.Any(rf => rf.Contains("brute force", StringComparison.OrdinalIgnoreCase)))
        {
            securityAdvice.AddRange(new[]
            {
                "Change your password immediately - brute force attack detected",
                "Review and revoke access for any unrecognized devices",
                "Consider using a more complex password or passphrase"
            });
        }

        if (riskFactors.Any(rf => rf.Contains("new IP address", StringComparison.OrdinalIgnoreCase) || 
                                  rf.Contains("new geographic location", StringComparison.OrdinalIgnoreCase)))
        {
            securityAdvice.AddRange(new[]
            {
                "Verify this login was made by you from the new location",
                "If unrecognized, secure your account and change credentials immediately"
            });
        }

        if (riskFactors.Any(rf => rf.Contains("new device/browser", StringComparison.OrdinalIgnoreCase)))
        {
            securityAdvice.AddRange(new[]
            {
                "Verify this login was made from your device",
                "If using a public or shared device, ensure you log out completely"
            });
        }

        if (riskFactors.Any(rf => rf.Contains("unusual hours", StringComparison.OrdinalIgnoreCase)))
        {
            securityAdvice.Add("If this wasn't you logging in during unusual hours, secure your account immediately");
        }
    }

    private async Task CreateOrUpdateRiskSummaryAsync(
        IApplicationDbContext dbContext, 
        string userId, 
        string userName, 
        SecurityAnalysisResult analysisResult, 
        CancellationToken cancellationToken)
    {
        var existingSummary = await dbContext.UserLoginRiskSummaries
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        var description = string.Join("; ", analysisResult.RiskFactors);
        var advice = string.Join("; ", analysisResult.SecurityAdvice);

        if (existingSummary != null)
        {
            // Only update if there's a meaningful change
            if (existingSummary.RiskLevel != analysisResult.RiskLevel || 
                existingSummary.RiskScore != analysisResult.RiskScore ||
                existingSummary.Description != description)
            {
                existingSummary.RiskLevel = analysisResult.RiskLevel;
                existingSummary.RiskScore = analysisResult.RiskScore;
                existingSummary.Description = description;
                existingSummary.Advice = advice;
                existingSummary.LastModified = DateTime.UtcNow;
            }
        }
        else
        {
            var newSummary = new UserLoginRiskSummary
            {
                UserId = userId,
                UserName = userName,
                RiskLevel = analysisResult.RiskLevel,
                RiskScore = analysisResult.RiskScore,
                Description = description,
                Advice = advice
            };

            await dbContext.UserLoginRiskSummaries.AddAsync(newSummary, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
} 