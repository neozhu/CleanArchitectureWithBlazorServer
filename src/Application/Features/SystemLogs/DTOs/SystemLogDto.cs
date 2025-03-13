﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;

public class SystemLogDto
{
    [Description("Id")] public int Id { get; set; }

    [Description("Message")] public string? Message { get; set; }

    [Description("Message Template")] public string? MessageTemplate { get; set; }

    [Description("Level")] public string Level { get; set; } = default!;

    [Description("Datetime")] public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    [Description("Exception")] public string? Exception { get; set; }

    [Description("User name")] public string? UserName { get; set; }

    [Description("Client IP")] public string? ClientIP { get; set; }

    [Description("Client Agent")] public string? ClientAgent { get; set; }

    [Description("Properties")] public string? Properties { get; set; }

    [Description("Log Event")] public string? LogEvent { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SystemLog, SystemLogDto>().ReverseMap();
        }
    }
}