// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace CleanArchitecture.Blazor.Application.Common.Security;

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

    [DisplayName("Navigation Menu Permissions")]
    [Description("Set permissions for navigation menu")]
    public static class NavigationMenu
    {
        [Description("Allows viewing the navigation menu")]
        public const string View = "Permissions.NavigationMenu.View";
    }

    [DisplayName("Hangfire Permissions")]
    [Description("Set permissions for Hangfire dashboard")]
    public static class Hangfire
    {
        [Description("Allows viewing Hangfire dashboard")]
        public const string View = "Permissions.Hangfire.View";
    }
} 