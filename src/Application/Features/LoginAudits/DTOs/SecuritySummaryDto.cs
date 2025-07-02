// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;

[Description("Security Summary")]
public class SecuritySummaryDto
{
    [Description("User Id")] public string UserId { get; set; } = string.Empty;
    [Description("Risk Level")] public SecurityRiskLevel RiskLevel { get; set; }
    [Description("Has Security Warnings")] public bool HasSecurityWarnings { get; set; }
    [Description("Last Login Date")] public DateTime? LastLoginDate { get; set; }
    [Description("New Devices Count")] public int NewDevicesCount { get; set; }
    [Description("New IP Addresses Count")] public int NewIpAddressesCount { get; set; }
    [Description("Failed Logins Last 7 Days")] public int FailedLoginsLast7Days { get; set; }
    [Description("Should Change Password")] public bool ShouldChangePassword { get; set; }
} 