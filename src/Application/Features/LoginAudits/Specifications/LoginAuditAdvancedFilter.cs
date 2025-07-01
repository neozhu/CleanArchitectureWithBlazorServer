﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Specifications;

public enum LoginAuditListView
{
    [Description("All")]
    All,
    [Description("My Login History")]
    My,
    [Description("Created Today")]
    TODAY,
    [Description("View of the last 30 days")]
    LAST_30_DAYS,
}

public class LoginAuditAdvancedFilter : PaginationFilter
{
    public LoginAuditListView ListView { get; set; } = LoginAuditListView.LAST_30_DAYS;
    public UserProfile? CurrentUser { get; set; }
    public bool? Success { get; set; }
    public string? Provider { get; set; }
}
