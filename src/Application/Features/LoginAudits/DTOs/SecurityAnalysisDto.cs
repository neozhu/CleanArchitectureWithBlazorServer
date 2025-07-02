// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;

[Description("Security Analysis")]
public class SecurityAnalysisDto
{
    [Description("User Id")] public string UserId { get; set; } = string.Empty;
    [Description("User Name")] public string UserName { get; set; } = string.Empty;
    [Description("Analysis Date")] public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;
    [Description("Overall Risk Level")] public SecurityRiskLevel OverallRiskLevel { get; set; }
    [Description("Risk Score")] public int RiskScore { get; set; }
    [Description("Security Threats")] public List<SecurityThreatDto> SecurityThreats { get; set; } = new();
    [Description("Recommendations")] public List<string> Recommendations { get; set; } = new();
    [Description("Analysis Period Days")] public int AnalysisPeriodDays { get; set; }
    [Description("Should Change Password")] public bool ShouldChangePassword { get; set; }
}

[Description("Security Threat")]
public class SecurityThreatDto
{
    [Description("Threat Type")] public SecurityThreatType ThreatType { get; set; }
    [Description("Risk Level")] public SecurityRiskLevel RiskLevel { get; set; }
    [Description("Description")] public string Description { get; set; } = string.Empty;
    [Description("Details")] public string Details { get; set; } = string.Empty;
    [Description("First Detected")] public DateTime FirstDetected { get; set; }
    [Description("Last Detected")] public DateTime LastDetected { get; set; }
    [Description("Occurrence Count")] public int OccurrenceCount { get; set; }
    [Description("Additional Data")] public Dictionary<string, object> AdditionalData { get; set; } = new();
} 