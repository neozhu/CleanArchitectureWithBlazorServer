// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Login Audit Permissions")]
    [Description("Set permissions for login audit operations")]
    public static class LoginAudits
    {
        [Description("Allows viewing login audit details")]
        public const string View = "Permissions.LoginAudits.View";

        [Description("Allows searching for login audit records")]
        public const string Search = "Permissions.LoginAudits.Search";

        [Description("Allows exporting login audit records")]
        public const string Export = "Permissions.LoginAudits.Export";

        [Description("Allows viewing all users' login audits")]
        public const string ViewAll = "Permissions.LoginAudits.ViewAll";
    }
}

public class LoginAuditsAccessRights
{
    public bool View { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool ViewAll { get; set; }
} 
