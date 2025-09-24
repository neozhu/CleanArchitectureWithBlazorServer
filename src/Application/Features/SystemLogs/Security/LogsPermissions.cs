// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Log Permissions")]
    [Description("Set permissions for log operations")]
    public static class Logs
    {
        [Description("Allows viewing log details")]
        public const string View = "Permissions.Logs.View";

        [Description("Allows searching for log records")]
        public const string Search = "Permissions.Logs.Search";

        [Description("Allows exporting log records")]
        public const string Export = "Permissions.Logs.Export";

        [Description("Allows purging log records")]
        public const string Purge = "Permissions.Logs.Purge";
    }
}

public class LogsAccessRights
{
    public bool View { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Purge { get; set; }
} 
