// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Tenant Permissions")]
    [Description("Set permissions for tenant operations")]
    public static class Tenants
    {
        [Description("Allows viewing tenant details")]
        public const string View = "Permissions.Tenants.View";

        [Description("Allows creating new tenants")]
        public const string Create = "Permissions.Tenants.Create";

        [Description("Allows modifying existing tenants")]
        public const string Edit = "Permissions.Tenants.Edit";

        [Description("Allows deleting tenants")]
        public const string Delete = "Permissions.Tenants.Delete";

        [Description("Allows searching for tenant records")]
        public const string Search = "Permissions.Tenants.Search";
    }
}

public class TenantsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
} 
