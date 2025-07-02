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
        public const string View = "Permissions.Contacts.View";

        [Description("Allows creating new contact records")]
        public const string Create = "Permissions.Contacts.Create";

        [Description("Allows modifying existing contact details")]
        public const string Edit = "Permissions.Contacts.Edit";

        [Description("Allows deleting contact records")]
        public const string Delete = "Permissions.Contacts.Delete";

        [Description("Allows printing contact details")]
        public const string Print = "Permissions.Contacts.Print";

        [Description("Allows searching for contact records")]
        public const string Search = "Permissions.Contacts.Search";

        [Description("Allows exporting contact records")]
        public const string Export = "Permissions.Contacts.Export";

        [Description("Allows importing contact records")]
        public const string Import = "Permissions.Contacts.Import";
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

