// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;

[Description("User Login Risk Summary")]
public class UserLoginRiskSummaryDto
{
    [Description("Id")] public int Id { get; set; }
    [Description("User Id")] public string UserId { get; set; } = string.Empty;
    [Description("User Name")] public string UserName { get; set; } = string.Empty;
    [Description("Risk Level")] public SecurityRiskLevel RiskLevel { get; set; }
    [Description("Risk Score")] public int RiskScore { get; set; }
    [Description("Description")] public string? Description { get; set; }
    [Description("Advice")] public string? Advice { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Identity.UserLoginRiskSummary, UserLoginRiskSummaryDto>().ReverseMap();
        }
    }
} 
