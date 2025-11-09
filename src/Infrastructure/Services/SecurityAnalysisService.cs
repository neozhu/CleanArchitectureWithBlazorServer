// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class SecurityAnalysisService : ISecurityAnalysisService
{
   
    private readonly ILogger<SecurityAnalysisService> _logger;
    private readonly IFusionCache _fusionCache;
    private readonly SecurityAnalysisOptions _options;
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IStringLocalizer<SecurityAnalysisService> _localizer;
    
    // Removed static dictionary to avoid unbounded growth & per-process divergence.
    // Use FusionCache short TTL entries (per-IP throttle) instead.
    private const string IpThrottlePrefix = "ip-analysis-throttle:"; // key = prefix + normalizedIp

    public SecurityAnalysisService(
        ILogger<SecurityAnalysisService> logger,
        IFusionCache fusionCache,
        IOptions<SecurityAnalysisOptions> options,
        IApplicationDbContextFactory dbContextFactory,
        IStringLocalizer<SecurityAnalysisService> localizer)
    {
        _logger = logger;
        _fusionCache = fusionCache;
        _options = options.Value;
        _dbContextFactory = dbContextFactory;
        _localizer = localizer;
    }

    public async Task AnalyzeUserSecurityAsync(LoginAudit loginAudit, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
            var analysisResult = await PerformSecurityAnalysisAsync(db, loginAudit, cancellationToken);
            
            await CreateOrUpdateRiskSummaryAsync(db, loginAudit.UserId, loginAudit.UserName, 
                analysisResult, cancellationToken);

            // Invalidate tags: always invalidate user-specific collections (loginaudits, userloginrisksummary)
            // Avoid clearing aggregated 'statistics' unless the risk summary changed significantly
            var tagsToRemove = new List<string>();
            tagsToRemove.Add("loginaudits");
            tagsToRemove.Add("userloginrisksummary");
            // Heuristic: critical or high risk or change in risk level triggers statistics invalidation.
            if (analysisResult.RiskLevel != SecurityRiskLevel.Low)
            {
                tagsToRemove.Add("statistics");
            }
            var tagRemovalTasks = tagsToRemove.Distinct()
                .Select(tag => _fusionCache.RemoveByTagAsync(tag, token: cancellationToken).AsTask());
            await Task.WhenAll(tagRemovalTasks);
            
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
        analysisResult.TriggeredRules = ruleResults.Where(r => r.IsTriggered).Select(r => r.RuleName).ToHashSet();
        
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
            result.Factors.Add(_localizer["AccountBruteForceFactor", userFailuresInWindow, _options.BruteForceWindowMinutes]);
        }

        // Analyze IP-level brute force with FusionCache throttle (multi-instance friendly if distributed layer configured)
        if (!string.IsNullOrWhiteSpace(currentLogin.IpAddress))
        {
            var normalizedIp = SecurityAnalysisHeuristics.NormalizeIpForThrottle(currentLogin.IpAddress);
            var throttleKey = IpThrottlePrefix + normalizedIp;
            // Try get a throttle marker; if absent, perform analysis and set marker with short TTL.
            var throttle = await _fusionCache.TryGetAsync<bool>(throttleKey, token: cancellationToken);
            if (!throttle.HasValue)
            {
                var ipAnalysisResult = await AnalyzeIpBruteForceAsync(dbContext, currentLogin, bruteForceWindow, cancellationToken);
                if (ipAnalysisResult.IsTriggered)
                {
                    result.Score += ipAnalysisResult.Score;
                    result.Factors.AddRange(ipAnalysisResult.Factors);
                }
                // Set marker with TTL (configurable? use 5 min default, could expose via options later)
                await _fusionCache.SetAsync(throttleKey, true, TimeSpan.FromMinutes(5), token: cancellationToken);
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
            result.Factors.Add(_localizer["IpBruteForceFactor", totalIpFailures, currentLogin.IpAddress ?? string.Empty, distinctUsersTargeted]);
        }

        return result;
    }

    private RiskAnalysisRuleResult AnalyzeNewDeviceOrLocation(List<LoginAudit> userLoginAudits, LoginAudit currentLogin)
    {
        var result = new RiskAnalysisRuleResult { RuleName = "NewDeviceOrLocation" };

        if (!currentLogin.Success) return result;

        var newFactors = new List<string>();

        // Normalize comparison tokens
    var currentIpCidr24 = SecurityAnalysisHeuristics.NormalizeIpForHeuristic(currentLogin.IpAddress);
    var currentRegionLevel = SecurityAnalysisHeuristics.ExtractRegionHierarchy(currentLogin.Region, out var regionDisplay);
    var currentUaCore = SecurityAnalysisHeuristics.ExtractUserAgentCore(currentLogin.BrowserInfo);

    bool seenIpSubnet = !string.IsNullOrEmpty(currentIpCidr24) && userLoginAudits.Any(x => x.Success && SecurityAnalysisHeuristics.NormalizeIpForHeuristic(x.IpAddress) == currentIpCidr24 && x.Id != currentLogin.Id);
    bool seenRegionLevel = !string.IsNullOrEmpty(currentRegionLevel) && userLoginAudits.Any(x => x.Success && SecurityAnalysisHeuristics.ExtractRegionHierarchy(x.Region, out _) == currentRegionLevel && x.Id != currentLogin.Id);
    bool seenUaCore = !string.IsNullOrEmpty(currentUaCore) && userLoginAudits.Any(x => x.Success && SecurityAnalysisHeuristics.ExtractUserAgentCore(x.BrowserInfo) == currentUaCore && x.Id != currentLogin.Id);

        if (!seenIpSubnet && !string.IsNullOrEmpty(currentIpCidr24))
        {
            newFactors.Add(_localizer["NewIpFactor", currentIpCidr24]);
        }
        if (!seenRegionLevel && !string.IsNullOrEmpty(regionDisplay))
        {
            newFactors.Add(_localizer["NewRegionFactor", regionDisplay]);
        }
        if (!seenUaCore && !string.IsNullOrEmpty(currentUaCore))
        {
            newFactors.Add(_localizer["NewBrowserFactor", currentUaCore]);
        }

        if (newFactors.Any())
        {
            result.Score = _options.NewDeviceLocationScore;
            result.Factors.Add(_localizer["LoginFromFactors", string.Join(", ", newFactors)]);
        }

        return result;
    }

    // ---------------------------------------
    // Helper methods for relaxed heuristics
    // ---------------------------------------
    // helper methods moved to SecurityAnalysisHeuristics for testability

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
            result.Factors.Add(_localizer["UnusualHoursFactor", currentLogin.LoginTimeUtc.ToString("HH:mm")] );
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
                    _localizer["Advice_Critical_ImmediateAction"].Value,
                    _localizer["Advice_Critical_ChangePassword"].Value,
                    _localizer["Advice_Critical_Enable2FA"].Value,
                    _localizer["Advice_Critical_ReviewDevices"].Value,
                    _localizer["Advice_Critical_ContactSecurity"].Value,
                });
                break;
                
            case SecurityRiskLevel.High:
                securityAdvice.AddRange(new[]
                {
                    _localizer["Advice_High_ReviewActivity"].Value,
                    _localizer["Advice_High_ChangePasswordConsider"].Value,
                    _localizer["Advice_High_Enable2FA"].Value,
                    _localizer["Advice_High_MonitorAccount"].Value,
                });
                break;
                
            case SecurityRiskLevel.Medium:
                securityAdvice.AddRange(new[]
                {
                    _localizer["Advice_Medium_Monitor"].Value,
                    _localizer["Advice_Medium_Enable2FAConsider"].Value,
                    _localizer["Advice_Medium_ReviewLoginHistory"].Value,
                });
                break;
                
            case SecurityRiskLevel.Low:
                securityAdvice.Add(_localizer["Advice_Low_Normal"].Value);
                break;
        }

        // Specific advice based on risk factors
        if (analysisResult.TriggeredRules.Contains("ConcentratedFailures") || analysisResult.TriggeredRules.Contains("IpBruteForce"))
        {
            securityAdvice.AddRange(new[]
            {
                _localizer["Advice_Factor_BruteForce_ChangePassword"].Value,
                _localizer["Advice_Factor_BruteForce_ReviewDevices"].Value,
                _localizer["Advice_Factor_BruteForce_StrongerPassword"].Value,
            });
        }

        if (analysisResult.TriggeredRules.Contains("NewDeviceOrLocation"))
        {
            securityAdvice.AddRange(new[]
            {
                _localizer["Advice_Factor_NewLocation_Verify"].Value,
                _localizer["Advice_Factor_NewLocation_SecureIfUnrecognized"].Value,
            });
        }

        if (analysisResult.TriggeredRules.Contains("NewDeviceOrLocation"))
        {
            securityAdvice.AddRange(new[]
            {
                _localizer["Advice_Factor_NewDevice_Verify"].Value,
                _localizer["Advice_Factor_NewDevice_PublicDevice"].Value,
            });
        }

        if (analysisResult.TriggeredRules.Contains("UnusualTimeLogin"))
        {
            securityAdvice.Add(_localizer["Advice_Factor_UnusualHours_Secure"].Value);
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
                existingSummary.LastModifiedAt = DateTime.UtcNow;
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
