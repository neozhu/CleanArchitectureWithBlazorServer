// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;

public class RiskSummaryStatisticsDto
{
    public int TotalUsers { get; set; }
    public int LowRiskUsers { get; set; }
    public int MediumRiskUsers { get; set; }
    public int HighRiskUsers { get; set; }
    public int CriticalRiskUsers { get; set; }
    public double AverageRiskScore { get; set; }
    public int TotalRiskAnalyses { get; set; }
    public DateTime? LastAnalysisTime { get; set; }
}
