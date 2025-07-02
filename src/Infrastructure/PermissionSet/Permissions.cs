// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    /// <summary>
    ///     Returns a list of Permissions.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                     c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add((string)propertyValue);
        }

        return permissions;
    }


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

    [DisplayName("Picklist Permissions")]
    [Description("Set permissions for picklist operations")]
    public static class PicklistSets
    {
        [Description("Allows viewing picklist set details")]
        public const string View = "Permissions.PicklistSets.View";

        [Description("Allows creating new picklist sets")]
        public const string Create = "Permissions.PicklistSets.Create";

        [Description("Allows modifying existing picklist sets")]
        public const string Edit = "Permissions.PicklistSets.Edit";

        [Description("Allows deleting picklist sets")]
        public const string Delete = "Permissions.PicklistSets.Delete";

        [Description("Allows searching for picklist set records")]
        public const string Search = "Permissions.PicklistSets.Search";

        [Description("Allows exporting picklist set records")]
        public const string Export = "Permissions.PicklistSets.Export";

        [Description("Allows importing picklist set records")]
        public const string Import = "Permissions.PicklistSets.Import";
    }

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
    }

    [DisplayName("Role Permissions")]
    [Description("Set permissions for role operations")]
    public static class Roles
    {
        [Description("Allows viewing role details")]
        public const string View = "Permissions.Roles.View";

        [Description("Allows creating new roles")]
        public const string Create = "Permissions.Roles.Create";

        [Description("Allows modifying existing role details")]
        public const string Edit = "Permissions.Roles.Edit";

        [Description("Allows deleting roles")]
        public const string Delete = "Permissions.Roles.Delete";

        [Description("Allows searching for role records")]
        public const string Search = "Permissions.Roles.Search";

        [Description("Allows exporting role records")]
        public const string Export = "Permissions.Roles.Export";

        [Description("Allows importing role records")]
        public const string Import = "Permissions.Roles.Import";

        [Description("Allows managing permissions for roles")]
        public const string ManagePermissions = "Permissions.Roles.ManagePermissions";

        [Description("Allows managing role-specific navigation menus")]
        public const string ManageNavigation = "Permissions.Roles.ManageNavigation";
    }

    [DisplayName("Tenant Permissions")]
    [Description("Set permissions for tenant operations")]
    public static class Tenants
    {
        [Description("Allows viewing tenant details")]
        public const string View = "Permissions.Tenants.View";

        [Description("Allows creating new tenants")]
        public const string Create = "Permissions.Tenants.Create";

        [Description("Allows modifying existing tenant details")]
        public const string Edit = "Permissions.Tenants.Edit";

        [Description("Allows deleting tenant records")]
        public const string Delete = "Permissions.Tenants.Delete";

        [Description("Allows searching for tenant records")]
        public const string Search = "Permissions.Tenants.Search";
    }

    [DisplayName("Dashboard Permissions")]
    [Description("Set permissions for dashboard operations")]
    public static class Dashboards
    {
        [Description("Allows viewing dashboard data")]
        public const string View = "Permissions.Dashboards.View";
    }


}

public class PicklistSetsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
}
public class LogsAccessRights
{
    public bool View { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Purge { get; set; }
}

public class RolesAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
    public bool ManagePermissions { get; set; }
    public bool ManageNavigation { get; set; }
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
}

public class AuditTrailsAccessRights
{
    public bool View { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
}

public class LoginAuditsAccessRights
{
    public bool View { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool ViewAll { get; set; }
}
public class TenantsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
}
public class DashboardsAccessRights
{
    public bool View { get; set; }
}