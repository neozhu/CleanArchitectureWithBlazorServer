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
    [Description("Set permissions for audit operations.")]
    public static class AuditTrails
    {
        public const string View = "Permissions.AuditTrails.View";
        public const string Search = "Permissions.AuditTrails.Search";
        public const string Export = "Permissions.AuditTrails.Export";
    }

    [DisplayName("Log Permissions")]
    [Description("Set permissions for log operations.")]
    public static class Logs
    {
        public const string View = "Permissions.Logs.View";
        public const string Search = "Permissions.Logs.Search";
        public const string Export = "Permissions.Logs.Export";
        public const string Purge = "Permissions.Logs.Purge";
    }


    [DisplayName("Picklist Permissions")]
    [Description("Set permissions for picklist operations.")]
    public static class PicklistSets
    {
        public const string View = "Permissions.PicklistSets.View";
        public const string Create = "Permissions.PicklistSets.Create";
        public const string Edit = "Permissions.PicklistSets.Edit";
        public const string Delete = "Permissions.PicklistSets.Delete";
        public const string Search = "Permissions.PicklistSets.Search";
        public const string Export = "Permissions.PicklistSets.Export";
        public const string Import = "Permissions.PicklistSets.Import";
    }

    [DisplayName("User Permissions")]
    [Description("Set permissions for user operations.")]
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Search = "Permissions.Users.Search";
        public const string Import = "Permissions.Users.Import";
        public const string Export = "Permissions.Dictionaries.Export";
        public const string ManageRoles = "Permissions.Users.ManageRoles";
        public const string RestPassword = "Permissions.Users.RestPassword";
        public const string SendRestPasswordMail = "Permissions.Users.SendRestPasswordMail";
        public const string ManagePermissions = "Permissions.Users.Permissions";
        public const string Deactivation = "Permissions.Users.Activation/Deactivation";
    }

    [DisplayName("Role Permissions")]
    [Description("Set permissions for role operations.")]
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
        public const string Search = "Permissions.Roles.Search";
        public const string Export = "Permissions.Roles.Export";
        public const string Import = "Permissions.Roles.Import";
        public const string ManagePermissions = "Permissions.Roles.Permissions";
        public const string ManageNavigation = "Permissions.Roles.Navigation";
    }

    [DisplayName("Tenant Permissions")]
    [Description("Set permissions for tenant operations.")]
    public static class Tenants
    {
        public const string View = "Permissions.Tenants.View";
        public const string Create = "Permissions.Tenants.Create";
        public const string Edit = "Permissions.Tenants.Edit";
        public const string Delete = "Permissions.Tenants.Delete";
        public const string Search = "Permissions.Tenants.Search";
    }



    [DisplayName("Dashboard Permissions")]
    [Description("Set permissions for dashboard operations.")]
    public static class Dashboards
    {
        public const string View = "Permissions.Dashboards.View";
    }

    [DisplayName("Job Permissions")]
    [Description("Set permissions for job operations.")]
    public static class Hangfire
    {
        public const string View = "Permissions.Hangfire.View";
        public const string Jobs = "Permissions.Hangfire.Jobs";
    }
}