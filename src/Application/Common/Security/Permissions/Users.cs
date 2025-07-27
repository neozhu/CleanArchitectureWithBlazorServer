// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("User Permissions")]
    [Description("Set permissions for user operations")]
    public static class Users
    {
        [Description("Allows viewing user details")]
        public const string View = "Permissions.Users.View";

        [Description("Allows creating new user accounts")]
        public const string Create = "Permissions.Users.Create";

        [Description("Allows modifying existing user details")]
        public const string Edit = "Permissions.Users.Edit";

        [Description("Allows deleting user accounts")]
        public const string Delete = "Permissions.Users.Delete";

        [Description("Allows searching for user records")]
        public const string Search = "Permissions.Users.Search";

        [Description("Allows importing user records")]
        public const string Import = "Permissions.Users.Import";

        [Description("Allows exporting user records")]
        public const string Export = "Permissions.Users.Export";

        [Description("Allows managing user roles")]
        public const string ManageRoles = "Permissions.Users.ManageRoles";

        [Description("Allows resetting user passwords")]
        public const string RestPassword = "Permissions.Users.RestPassword";

        [Description("Allows sending password reset emails")]
        public const string SendRestPasswordMail = "Permissions.Users.SendRestPasswordMail";

        [Description("Allows managing user permissions")]
        public const string ManagePermissions = "Permissions.Users.ManagePermissions";

        [Description("Allows deactivating user accounts")]
        public const string Deactivation = "Permissions.Users.Deactivation";

        [Description("Allows viewing users' online status")]
        public const string ViewOnlineStatus = "Permissions.Users.ViewOnlineStatus";

        [Description("Allows switching between available tenants")]
        public const string SwitchTenants = "Permissions.Users.SwitchTenants";

        [Description("Allows switching to any tenant (admin privilege)")]
        public const string SwitchToAnyTenant = "Permissions.Users.SwitchToAnyTenant";
    }
}

public class UsersAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Import { get; set; }
    public bool Export { get; set; }
    public bool ManageRoles { get; set; }
    public bool RestPassword { get; set; }
    public bool SendRestPasswordMail { get; set; }
    public bool ManagePermissions { get; set; }
    public bool Deactivation { get; set; }
    public bool ViewOnlineStatus { get; set; }
    public bool SwitchTenants { get; set; }
    public bool SwitchToAnyTenant { get; set; }
} 