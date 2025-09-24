// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Dashboard Permissions")]
    [Description("Set permissions for dashboard operations")]
    public static class Dashboards
    {
        [Description("Allows viewing dashboard details")]
        public const string View = "Permissions.Dashboards.View";
    }
}

public class DashboardsAccessRights
{
    public bool View { get; set; }
} 
