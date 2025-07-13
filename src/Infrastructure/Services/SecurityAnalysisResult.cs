// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

/// <summary>
/// Represents the result of a security risk analysis
/// </summary>
public class SecurityAnalysisResult
{
    public int RiskScore { get; set; }
    public SecurityRiskLevel RiskLevel { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public List<string> SecurityAdvice { get; set; } = new();
    
    /// <summary>
    /// Detailed breakdown of risk scores by category
    /// </summary>
    public Dictionary<string, int> RiskScoreBreakdown { get; set; } = new();
}

/// <summary>
/// Individual risk analysis rule result
/// </summary>
public class RiskAnalysisRuleResult
{
    public string RuleName { get; set; } = string.Empty;
    public int Score { get; set; }
    public List<string> Factors { get; set; } = new();
    public bool IsTriggered => Score > 0;
}
