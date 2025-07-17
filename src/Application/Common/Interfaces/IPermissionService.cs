// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Permission service interface for Clean Architecture compliance
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Check if the current user has the specified permission
    /// </summary>
    /// <param name="permission">Permission to check</param>
    /// <returns>True if user has permission, false otherwise</returns>
    Task<bool> HasPermissionAsync(string permission);

    /// <summary>
    /// Get access rights for a specific type
    /// </summary>
    /// <typeparam name="T">Access rights type</typeparam>
    /// <returns>Access rights object with populated permissions</returns>
    Task<T> GetAccessRightsAsync<T>() where T : new();

    /// <summary>
    /// Get all permissions for the current user
    /// </summary>
    /// <returns>List of permissions</returns>
    Task<List<string>> GetUserPermissionsAsync();

    /// <summary>
    /// Get all available permissions in the system
    /// </summary>
    /// <returns>List of all registered permissions</returns>
    List<string> GetAllPermissions();
}
