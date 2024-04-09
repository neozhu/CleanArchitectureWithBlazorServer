// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Customers")]
    [Description("Customers Permissions")]
    public static class Customers
    {
        public const string View = "Permissions.Customers.View";
        public const string Create = "Permissions.Customers.Create";
        public const string Edit = "Permissions.Customers.Edit";
        public const string Delete = "Permissions.Customers.Delete";
        public const string Search = "Permissions.Customers.Search";
        public const string Export = "Permissions.Customers.Export";
        public const string Import = "Permissions.Customers.Import";
    }
}