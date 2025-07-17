// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
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