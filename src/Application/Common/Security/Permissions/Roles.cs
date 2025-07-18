// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Role Permissions")]
    [Description("Set permissions for role operations")]
    public static class Roles
    {
        [Description("Allows viewing role details")]
        public const string View = "Permissions.Roles.View";

        [Description("Allows creating new roles")]
        public const string Create = "Permissions.Roles.Create";

        [Description("Allows modifying existing roles")]
        public const string Edit = "Permissions.Roles.Edit";

        [Description("Allows deleting roles")]
        public const string Delete = "Permissions.Roles.Delete";

        [Description("Allows searching for role records")]
        public const string Search = "Permissions.Roles.Search";

        [Description("Allows importing role data")]
        public const string Import = "Permissions.Roles.Import";
        [Description("Allows exporting role data")]
        public const string Export= "Permissions.Roles.Export";

        [Description("Allows managing role permissions")]
        public const string ManagePermissions = "Permissions.Roles.ManagePermissions";

        [Description("Allows managing role claims")]
        public const string ManageClaimsInRole = "Permissions.Roles.ManageClaimsInRole";

        [Description("Allows managing users in role")]
        public const string ManageUsersInRole = "Permissions.Roles.ManageUsersInRole";

        [Description("Allows viewing role permissions")]
        public const string ViewPermissions = "Permissions.Roles.ViewPermissions";

        [Description("Allows viewing role claims")]
        public const string ViewClaimsInRole = "Permissions.Roles.ViewClaimsInRole";

        [Description("Allows viewing users in role")]
        public const string ViewUsersInRole = "Permissions.Roles.ViewUsersInRole";
    }
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
    public bool ManageClaimsInRole { get; set; }
    public bool ManageUsersInRole { get; set; }
    public bool ViewPermissions { get; set; }
    public bool ViewClaimsInRole { get; set; }
    public bool ViewUsersInRole { get; set; }
} 