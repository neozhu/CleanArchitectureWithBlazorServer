// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{

    [DisplayName("Contact Permissions")]
    [Description("Set permissions for contact operations")]
    public static class Contacts
    {
        [Description("Allows viewing contact details")]
        public const string View = "PermissionsContactsView";

        [Description("Allows creating new contact records")]
        public const string Create = "PermissionsContactsCreate";

        [Description("Allows modifying existing contact details")]
        public const string Edit = "PermissionsContactsEdit";

        [Description("Allows deleting contact records")]
        public const string Delete = "PermissionsContactsDelete";

        [Description("Allows printing contact details")]
        public const string Print = "PermissionsContactsPrint";

        [Description("Allows searching for contact records")]
        public const string Search = "PermissionsContactsSearch";

        [Description("Allows exporting contact records")]
        public const string Export = "PermissionsContactsExport";

        [Description("Allows importing contact records")]
        public const string Import = "PermissionsContactsImport";
    }
}

public class ContactsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Print { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
}

