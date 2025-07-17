using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Interface for permission helper operations.
/// </summary>
public interface IPermissionHelper
{
    /// <summary>
    /// Gets all permissions for a user by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The list of permission models.</returns>
    Task<IList<PermissionModel>> GetAllPermissionsByUserId(string userId);
    
    /// <summary>
    /// Gets all permissions for a role by role ID.
    /// </summary>
    /// <param name="roleId">The role ID.</param>
    /// <returns>The list of permission models.</returns>
    Task<IList<PermissionModel>> GetAllPermissionsByRoleId(string roleId);
} 