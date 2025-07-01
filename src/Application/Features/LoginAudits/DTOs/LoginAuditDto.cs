// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;

[Description("Login Audits")]
public class LoginAuditDto
{
    [Description("Id")] public string Id { get; set; } = string.Empty;
    [Description("Login Time")] public DateTime LoginTimeUtc { get; set; }
    [Description("User Id")] public string UserId { get; set; } = string.Empty;
    [Description("User Name")] public string UserName { get; set; } = string.Empty;
    [Description("IP Address")] public string? IpAddress { get; set; }
    [Description("Browser Info")] public string? BrowserInfo { get; set; }
    [Description("Region")] public string? Region { get; set; }
    [Description("Provider")] public string? Provider { get; set; }
    [Description("Success")] public bool Success { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Identity.LoginAudit, LoginAuditDto>().ReverseMap();
        }
    }
}
