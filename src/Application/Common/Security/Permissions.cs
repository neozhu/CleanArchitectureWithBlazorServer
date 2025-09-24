// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    /// <summary>
    ///     Returns a list of Permissions by scanning all assemblies for Permissions classes.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        
        // Scan current assembly for all classes named "Permissions" (both in Common and Features)
        var assembly = Assembly.GetExecutingAssembly();
        var permissionClasses = assembly.GetTypes()
            .Where(t => t.Name == "Permissions" && t.IsClass && t.IsAbstract && t.IsSealed)
            .ToList();

        foreach (var permissionClass in permissionClasses)
        {
            foreach (var nestedType in permissionClass.GetNestedTypes())
            {
                var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var field in fields)
                {
                    var propertyValue = field.GetValue(null);
                    if (propertyValue is string permission)
                        permissions.Add(permission);
                }
            }
        }

        return permissions.Distinct().ToList();
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
