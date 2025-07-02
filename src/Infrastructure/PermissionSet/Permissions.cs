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
        public const string View = "PermissionsAuditTrailsView";

        [Description("Allows searching for audit trail records")]
        public const string Search = "PermissionsAuditTrailsSearch";

        [Description("Allows exporting audit trail records")]
        public const string Export = "PermissionsAuditTrailsExport";
    }

    [DisplayName("Log Permissions")]
    [Description("Set permissions for log operations")]
    public static class Logs
    {
        [Description("Allows viewing log details")]
        public const string View = "PermissionsLogsView";

        [Description("Allows searching for log records")]
        public const string Search = "PermissionsLogsSearch";

        [Description("Allows exporting log records")]
        public const string Export = "PermissionsLogsExport";

        [Description("Allows purging log records")]
        public const string Purge = "PermissionsLogsPurge";
    }

    [DisplayName("Login Audit Permissions")]
    [Description("Set permissions for login audit operations")]
    public static class LoginAudits
    {
        [Description("Allows viewing login audit details")]
        public const string View = "PermissionsLoginAuditsView";

        [Description("Allows searching for login audit records")]
        public const string Search = "PermissionsLoginAuditsSearch";

        [Description("Allows exporting login audit records")]
        public const string Export = "PermissionsLoginAuditsExport";

        [Description("Allows viewing all users' login audits")]
        public const string ViewAll = "PermissionsLoginAuditsViewAll";
    }

    [DisplayName("Picklist Permissions")]
    [Description("Set permissions for picklist operations")]
    public static class PicklistSets
    {
        [Description("Allows viewing picklist set details")]
        public const string View = "PermissionsPicklistSetsView";

        [Description("Allows creating new picklist sets")]
        public const string Create = "PermissionsPicklistSetsCreate";

        [Description("Allows modifying existing picklist sets")]
        public const string Edit = "PermissionsPicklistSetsEdit";

        [Description("Allows deleting picklist sets")]
        public const string Delete = "PermissionsPicklistSetsDelete";

        [Description("Allows searching for picklist set records")]
        public const string Search = "PermissionsPicklistSetsSearch";

        [Description("Allows exporting picklist set records")]
        public const string Export = "PermissionsPicklistSetsExport";

        [Description("Allows importing picklist set records")]
        public const string Import = "PermissionsPicklistSetsImport";
    }

    [DisplayName("User Permissions")]
    [Description("Set permissions for user operations")]
    public static class Users
    {
        [Description("Allows viewing user details")]
        public const string View = "PermissionsUsersView";

        [Description("Allows creating new user accounts")]
        public const string Create = "PermissionsUsersCreate";

        [Description("Allows modifying existing user details")]
        public const string Edit = "PermissionsUsersEdit";

        [Description("Allows deleting user accounts")]
        public const string Delete = "PermissionsUsersDelete";

        [Description("Allows searching for user records")]
        public const string Search = "PermissionsUsersSearch";

        [Description("Allows importing user records")]
        public const string Import = "PermissionsUsersImport";

        [Description("Allows exporting user records")]
        public const string Export = "PermissionsUsersExport";

        [Description("Allows managing user roles")]
        public const string ManageRoles = "PermissionsUsersManageRoles";

        [Description("Allows resetting user passwords")]
        public const string RestPassword = "PermissionsUsersRestPassword";

        [Description("Allows sending password reset emails")]
        public const string SendRestPasswordMail = "PermissionsUsersSendRestPasswordMail";

        [Description("Allows managing user permissions")]
        public const string ManagePermissions = "PermissionsUsersManagePermissions";

        [Description("Allows deactivating user accounts")]
        public const string Deactivation = "PermissionsUsersDeactivation";

        [Description("Allows viewing users' online status")]
        public const string ViewOnlineStatus = "PermissionsUsersViewOnlineStatus";
    }

    [DisplayName("Role Permissions")]
    [Description("Set permissions for role operations")]
    public static class Roles
    {
        [Description("Allows viewing role details")]
        public const string View = "PermissionsRolesView";

        [Description("Allows creating new roles")]
        public const string Create = "PermissionsRolesCreate";

        [Description("Allows modifying existing role details")]
        public const string Edit = "PermissionsRolesEdit";

        [Description("Allows deleting roles")]
        public const string Delete = "PermissionsRolesDelete";

        [Description("Allows searching for role records")]
        public const string Search = "PermissionsRolesSearch";

        [Description("Allows exporting role records")]
        public const string Export = "PermissionsRolesExport";

        [Description("Allows importing role records")]
        public const string Import = "PermissionsRolesImport";

        [Description("Allows managing permissions for roles")]
        public const string ManagePermissions = "PermissionsRolesManagePermissions";

        [Description("Allows managing role-specific navigation menus")]
        public const string ManageNavigation = "PermissionsRolesManageNavigation";
    }

    [DisplayName("Tenant Permissions")]
    [Description("Set permissions for tenant operations")]
    public static class Tenants
    {
        [Description("Allows viewing tenant details")]
        public const string View = "PermissionsTenantsView";

        [Description("Allows creating new tenants")]
        public const string Create = "PermissionsTenantsCreate";

        [Description("Allows modifying existing tenant details")]
        public const string Edit = "PermissionsTenantsEdit";

        [Description("Allows deleting tenant records")]
        public const string Delete = "PermissionsTenantsDelete";

        [Description("Allows searching for tenant records")]
        public const string Search = "PermissionsTenantsSearch";
    }

    [DisplayName("Dashboard Permissions")]
    [Description("Set permissions for dashboard operations")]
    public static class Dashboards
    {
        [Description("Allows viewing dashboard data")]
        public const string View = "PermissionsDashboardsView";
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