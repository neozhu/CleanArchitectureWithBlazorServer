namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Abstraction for loading and assigning permission claims to a specific security principal (user or role).
/// </summary>
public interface IPermissionAssignmentService
{
    /// <summary>
    /// Load all permissions for the given entity identifier (userId / roleId).
    /// </summary>
    /// <param name="entityId">Id of the user or role.</param>
    /// <returns>List of PermissionModel representing the permission/claim state.</returns>
    Task<IList<PermissionModel>> LoadAsync(string entityId);

    /// <summary>
    /// Toggle assignment of a single permission according to <see cref="PermissionModel.Assigned"/> flag.
    /// </summary>
    /// <param name="model">Permission model to assign/unassign.</param>
    Task AssignAsync(PermissionModel model);

    /// <summary>
    /// Assign or unassign a batch of permissions. The <see cref="PermissionModel.Assigned"/> state will be persisted.
    /// </summary>
    /// <param name="models">Enumerable of permission models.</param>
    Task AssignBulkAsync(IEnumerable<PermissionModel> models);
} 