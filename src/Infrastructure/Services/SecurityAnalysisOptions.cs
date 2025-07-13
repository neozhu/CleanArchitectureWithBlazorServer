// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Services;

/// <summary>
/// Configuration options for security analysis
/// </summary>
public class SecurityAnalysisOptions
{
    public const string SectionName = "SecurityAnalysis";
    
    /// <summary>
    /// Number of days to look back for historical login data (default: 30)
    /// </summary>
    public int HistoryDays { get; set; } = 30;
    
    /// <summary>
    /// Time window in minutes for detecting concentrated failures (default: 10)
    /// </summary>
    public int BruteForceWindowMinutes { get; set; } = 10;
    
    /// <summary>
    /// Minimum failed attempts from same user to trigger account brute force alert (default: 3)
    /// </summary>
    public int AccountBruteForceThreshold { get; set; } = 3;
    
    /// <summary>
    /// Minimum failed attempts from same IP to trigger IP brute force alert (default: 10)
    /// </summary>
    public int IpBruteForceThreshold { get; set; } = 10;
    
    /// <summary>
    /// Minimum number of different accounts targeted from same IP for IP brute force (default: 3)
    /// </summary>
    public int IpBruteForceAccountThreshold { get; set; } = 3;
    
    /// <summary>
    /// Risk score for account brute force detection (default: 50)
    /// </summary>
    public int AccountBruteForceScore { get; set; } = 50;
    
    /// <summary>
    /// Risk score for IP brute force detection (default: 50)
    /// </summary>
    public int IpBruteForceScore { get; set; } = 50;
    
    /// <summary>
    /// Risk score for new device/location login (default: 25)
    /// </summary>
    public int NewDeviceLocationScore { get; set; } = 25;
    
    /// <summary>
    /// Risk score for unusual time login (default: 15)
    /// </summary>
    public int UnusualTimeScore { get; set; } = 15;
    
    /// <summary>
    /// Start hour (UTC) for unusual time detection (default: 22)
    /// </summary>
    public int UnusualTimeStartHour { get; set; } = 22;
    
    /// <summary>
    /// End hour (UTC) for unusual time detection (default: 6)
    /// </summary>
    public int UnusualTimeEndHour { get; set; } = 6;
    
    /// <summary>
    /// Risk score threshold for Medium risk level (default: 40)
    /// </summary>
    public int MediumRiskThreshold { get; set; } = 40;
    
    /// <summary>
    /// Risk score threshold for High risk level (default: 60)
    /// </summary>
    public int HighRiskThreshold { get; set; } = 60;
    
    /// <summary>
    /// Risk score threshold for Critical risk level (default: 80)
    /// </summary>
    public int CriticalRiskThreshold { get; set; } = 80;
    
    /// <summary>
    /// Cache expiration in minutes for user risk summaries (default: 15)
    /// </summary>
    public int CacheExpirationMinutes { get; set; } = 15;
}
