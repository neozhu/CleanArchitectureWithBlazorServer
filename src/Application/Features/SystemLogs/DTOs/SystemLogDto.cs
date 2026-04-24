// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;

[Description("System Log")]
public class SystemLogDto
{
    [Display(Name ="Id")] public int Id { get; set; }

    [Display(Name = "Message")] public string? Message { get; set; }

    [Display(Name = "Message Template")] public string? MessageTemplate { get; set; }

    [Display(Name = "Level")] public string Level { get; set; } = default!;

    [Display(Name = "Datetime")] public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    [Display(Name = "Exception")] public string? Exception { get; set; }

    [Display(Name = "User name")] public string? UserName { get; set; }

    [Display(Name = "Client IP")] public string? ClientIP { get; set; }

    [Display(Name = "Client Agent")] public string? ClientAgent { get; set; }

    [Display(Name = "Properties")] public string? Properties { get; set; }

    [Display(Name = "Log Event")] public string? LogEvent { get; set; }}
