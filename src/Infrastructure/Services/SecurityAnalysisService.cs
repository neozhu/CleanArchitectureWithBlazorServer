// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class SecurityAnalysisService : ISecurityAnalysisService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SecurityAnalysisService> _logger;
    private readonly IFusionCache _fusionCache;

    public SecurityAnalysisService(
        IServiceScopeFactory scopeFactory,
        ILogger<SecurityAnalysisService> logger,
        IFusionCache fusionCache)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _fusionCache = fusionCache;
    }

    public async Task AnalyzeUserSecurityAsync(LoginAudit loginAudit, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create a new scope to get a fresh DbContext instance
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var userId = loginAudit.UserId;
            var userName = loginAudit.UserName;
            var currentTime = loginAudit.LoginTimeUtc;
            var thirtyDaysAgo = currentTime.AddDays(-30);

            // Get all login audits for this user in the last 30 days
            var userLoginAudits = await dbContext.LoginAudits
                .Where(x => x.UserId == userId && x.LoginTimeUtc >= thirtyDaysAgo)
                .OrderByDescending(x => x.LoginTimeUtc)
                .ToListAsync(cancellationToken);

            var riskScore = 0;
            var riskFactors = new List<string>();
            var securityAdvice = new List<string>();

            // Rule C1: Concentrated failures/brute force (Account or IP)
            var c1Score = await AnalyzeConcentratedFailuresAsync(dbContext, userLoginAudits, loginAudit, riskFactors, cancellationToken);
            riskScore += c1Score;

            // Rule C2: New device/new geographic successful login
            var c2Score = AnalyzeNewDeviceOrLocation(userLoginAudits, loginAudit, riskFactors);
            riskScore += c2Score;

            // Rule C3: Unusual time successful login
            var c3Score = AnalyzeUnusualTimeLogin(loginAudit, riskFactors);
            riskScore += c3Score;

            // Determine risk level based on score
            var riskLevel = DetermineRiskLevel(riskScore);

            // Generate advice based on risk factors
            GenerateSecurityAdvice(riskFactors, securityAdvice, riskLevel);

            // Create or update UserLoginRiskSummary
            await CreateOrUpdateRiskSummaryAsync(dbContext, userId, userName, riskLevel, riskScore, 
                string.Join("; ", riskFactors), string.Join("; ", securityAdvice), cancellationToken);

            // Invalidate cache for the user's risk summary
            await _fusionCache.RemoveAsync($"UserLoginRiskSummary_{userId}");

            _logger.LogInformation("Security analysis completed for user {UserId}. Risk Level: {RiskLevel}, Score: {RiskScore}", 
                userId, riskLevel, riskScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze account security for user {UserId}", loginAudit.UserId);
        }
    }

    private async Task<int> AnalyzeConcentratedFailuresAsync(IApplicationDbContext dbContext, List<LoginAudit> userLoginAudits, LoginAudit currentLogin, List<string> riskFactors, CancellationToken cancellationToken)
    {
        var score = 0;
        var tenMinutesAgo = currentLogin.LoginTimeUtc.AddMinutes(-10);

        // Check same UserId failures within 10 minutes
        var userFailuresInWindow = userLoginAudits
            .Where(x => !x.Success && x.LoginTimeUtc >= tenMinutesAgo && x.LoginTimeUtc <= currentLogin.LoginTimeUtc)
            .Count();

        if (userFailuresInWindow >= 3)
        {
            score += 50;
            riskFactors.Add($"Account brute force detected: {userFailuresInWindow} failed login attempts within 10 minutes");
        }

        // Check same IP failures within 10 minutes involving multiple users
        if (!string.IsNullOrEmpty(currentLogin.IpAddress))
        {
            var ipFailuresInWindow = await dbContext.LoginAudits
                .Where(x => !x.Success && 
                           x.IpAddress == currentLogin.IpAddress && 
                           x.LoginTimeUtc >= tenMinutesAgo && 
                           x.LoginTimeUtc <= currentLogin.LoginTimeUtc)
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            var totalIpFailures = await dbContext.LoginAudits
                .Where(x => !x.Success && 
                           x.IpAddress == currentLogin.IpAddress && 
                           x.LoginTimeUtc >= tenMinutesAgo && 
                           x.LoginTimeUtc <= currentLogin.LoginTimeUtc)
                .CountAsync(cancellationToken);

            if (totalIpFailures >= 10 && ipFailuresInWindow >= 3)
            {
                score += 50;
                riskFactors.Add($"IP-based brute force detected: {totalIpFailures} failed attempts from IP {currentLogin.IpAddress} targeting {ipFailuresInWindow} different accounts");
            }
        }

        return score;
    }

    private int AnalyzeNewDeviceOrLocation(List<LoginAudit> userLoginAudits, LoginAudit currentLogin, List<string> riskFactors)
    {
        var score = 0;

        if (!currentLogin.Success) return score;

        // Check for new IP address
        var hasSeenIpBefore = userLoginAudits
            .Any(x => x.Success && 
                     x.IpAddress == currentLogin.IpAddress && 
                     x.Id != currentLogin.Id);

        // Check for new region
        var hasSeenRegionBefore = userLoginAudits
            .Any(x => x.Success && 
                     x.Region == currentLogin.Region && 
                     x.Id != currentLogin.Id);

        // Check for new browser info
        var hasSeenBrowserBefore = userLoginAudits
            .Any(x => x.Success && 
                     x.BrowserInfo == currentLogin.BrowserInfo && 
                     x.Id != currentLogin.Id);

        if (!hasSeenIpBefore || !hasSeenRegionBefore || !hasSeenBrowserBefore)
        {
            score += 25;
            var newFactors = new List<string>();
            
            if (!hasSeenIpBefore && !string.IsNullOrEmpty(currentLogin.IpAddress))
                newFactors.Add($"new IP address ({currentLogin.IpAddress})");
            
            if (!hasSeenRegionBefore && !string.IsNullOrEmpty(currentLogin.Region))
                newFactors.Add($"new geographic location ({currentLogin.Region})");
            
            if (!hasSeenBrowserBefore && !string.IsNullOrEmpty(currentLogin.BrowserInfo))
                newFactors.Add($"new device/browser ({currentLogin.BrowserInfo})");

            riskFactors.Add($"Login from {string.Join(", ", newFactors)}");
        }

        return score;
    }

    private int AnalyzeUnusualTimeLogin(LoginAudit currentLogin, List<string> riskFactors)
    {
        var score = 0;

        if (!currentLogin.Success) return score;

        // For simplicity, assume UTC time and check if login occurred between 22:00 and 06:00 UTC
        // In a real implementation, you would convert to user's local timezone
        var loginHour = currentLogin.LoginTimeUtc.Hour;
        
        if (loginHour >= 22 || loginHour < 6)
        {
            score += 15;
            riskFactors.Add($"Login during unusual hours ({currentLogin.LoginTimeUtc:HH:mm} UTC)");
        }

        return score;
    }

    private SecurityRiskLevel DetermineRiskLevel(int riskScore)
    {
        return riskScore switch
        {
            >= 60 => SecurityRiskLevel.High,
            >= 40 => SecurityRiskLevel.Medium,
            _ => SecurityRiskLevel.Low
        };
    }

    private void GenerateSecurityAdvice(List<string> riskFactors, List<string> securityAdvice, SecurityRiskLevel riskLevel)
    {
        if (riskLevel == SecurityRiskLevel.High)
        {
            securityAdvice.Add("Immediately review recent account activity");
            securityAdvice.Add("Consider changing your password");
            securityAdvice.Add("Enable two-factor authentication if not already active");
        }
        else if (riskLevel == SecurityRiskLevel.Medium)
        {
            securityAdvice.Add("Monitor your account for unusual activity");
            securityAdvice.Add("Consider enabling two-factor authentication");
        }
        else
        {
            securityAdvice.Add("Your account security appears normal");
        }

        if (riskFactors.Any(rf => rf.Contains("brute force")))
        {
            securityAdvice.Add("Change your password immediately");
            securityAdvice.Add("Review and revoke access for any unrecognized devices");
        }

        if (riskFactors.Any(rf => rf.Contains("new IP address") || rf.Contains("new geographic location")))
        {
            securityAdvice.Add("Verify this login was made by you");
            securityAdvice.Add("If unrecognized, secure your account and change credentials");
        }

        if (riskFactors.Any(rf => rf.Contains("unusual hours")))
        {
            securityAdvice.Add("If this wasn't you, secure your account immediately");
        }
    }

    private async Task CreateOrUpdateRiskSummaryAsync(IApplicationDbContext dbContext, string userId, string userName, SecurityRiskLevel riskLevel, 
        int riskScore, string description, string advice, CancellationToken cancellationToken)
    {
        var existingSummary = await dbContext.UserLoginRiskSummaries
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (existingSummary != null)
        {
            // Update existing record
            existingSummary.RiskLevel = riskLevel;
            existingSummary.RiskScore = riskScore;
            existingSummary.Description = description;
            existingSummary.Advice = advice;
            existingSummary.LastModified = DateTime.UtcNow;
        }
        else
        {
            // Create new record
            var newSummary = new UserLoginRiskSummary
            {
                UserId = userId,
                UserName = userName,
                RiskLevel = riskLevel,
                RiskScore = riskScore,
                Description = description,
                Advice = advice
            };

            await dbContext.UserLoginRiskSummaries.AddAsync(newSummary, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
} 