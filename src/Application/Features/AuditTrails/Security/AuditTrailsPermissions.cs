// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Audit Permissions")]
    [Description("Set permissions for audit operations")]
    public static class AuditTrails
    {
        [Description("Allows viewing audit trail details")]
        public const string View = "Permissions.AuditTrails.View";

        [Description("Allows searching for audit trail records")]
        public const string Search = "Permissions.AuditTrails.Search";

        [Description("Allows exporting audit trail records")]
        public const string Export = "Permissions.AuditTrails.Export";
    }
}

public class AuditTrailsAccessRights
{
    public bool View { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
} 
