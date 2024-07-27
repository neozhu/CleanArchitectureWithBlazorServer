// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Contacts")]
    [Description("Contacts Permissions")]
    public static class Contacts
    {
        public const string View = "Permissions.Contacts.View";
        public const string Create = "Permissions.Contacts.Create";
        public const string Edit = "Permissions.Contacts.Edit";
        public const string Delete = "Permissions.Contacts.Delete";
        public const string Search = "Permissions.Contacts.Search";
        public const string Export = "Permissions.Contacts.Export";
        public const string Import = "Permissions.Contacts.Import";
    }
}

