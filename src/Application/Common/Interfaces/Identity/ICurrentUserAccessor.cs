namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface to access the current user's session information.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Gets the current session information of the user.
    /// </summary>
    SessionInfo? SessionInfo { get; }

    /// <summary>
    /// Gets the current user ID.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user name.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Gets the current user display name.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// Gets the current user tenant ID.
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Gets the current user profile picture data URL.
    /// </summary>
    string? ProfilePictureDataUrl { get; }

    /// <summary>
    /// Gets the current user presence status.
    /// </summary>
    UserPresence Status { get; }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user has the specified permission.
    /// </summary>
    /// <param name="permission">The permission to check.</param>
    /// <returns>True if the user has the permission; otherwise, false.</returns>
    Task<bool> HasPermissionAsync(string permission);

    /// <summary>
    /// Gets the current user's access rights for the specified type.
    /// </summary>
    /// <typeparam name="TAccessRights">The type of access rights to get.</typeparam>
    /// <returns>The access rights for the current user.</returns>
    Task<TAccessRights> GetAccessRightsAsync<TAccessRights>() where TAccessRights : class, new();
}
