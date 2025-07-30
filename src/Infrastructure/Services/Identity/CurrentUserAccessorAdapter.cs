using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Adapter to maintain backward compatibility with ICurrentUserAccessor while using the new IUserContextAccessor.
/// </summary>
public class CurrentUserAccessorAdapter : ICurrentUserAccessor
{
    private readonly IUserContextAccessor _userContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserAccessorAdapter"/> class.
    /// </summary>
    /// <param name="userContextAccessor">The user context accessor.</param>
    public CurrentUserAccessorAdapter(IUserContextAccessor userContextAccessor)
    {
        _userContextAccessor = userContextAccessor;
    }

    /// <summary>
    /// Gets the session information of the current user.
    /// </summary>
    public SessionInfo? SessionInfo
    {
        get
        {
            var context = _userContextAccessor.Current;
            if (context == null)
                return null;

            return new SessionInfo(
                context.UserId,
                context.UserName,
                context.UserName, // DisplayName
                "", // IPAddress
                context.TenantId,
                "", // ProfilePictureDataUrl
                UserPresence.Available
            );
        }
    }

    /// <summary>
    /// Gets the current user ID.
    /// </summary>
    public string? UserId => _userContextAccessor.Current?.UserId;

    /// <summary>
    /// Gets the current user name.
    /// </summary>
    public string? UserName => _userContextAccessor.Current?.UserName;

    /// <summary>
    /// Gets the current user display name.
    /// </summary>
    public string? DisplayName => _userContextAccessor.Current?.UserName;

    /// <summary>
    /// Gets the current user tenant ID.
    /// </summary>
    public string? TenantId => _userContextAccessor.Current?.TenantId;

    /// <summary>
    /// Gets the current user profile picture data URL.
    /// </summary>
    public string? ProfilePictureDataUrl => ""; // Not available in UserContext

    /// <summary>
    /// Gets the current user presence status.
    /// </summary>
    public UserPresence Status => UserPresence.Available;

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated => _userContextAccessor.Current != null;

    /// <summary>
    /// Checks if the current user has the specified permission.
    /// </summary>
    /// <param name="permission">The permission to check.</param>
    /// <returns>True if the user has the permission; otherwise, false.</returns>
    public Task<bool> HasPermissionAsync(string permission)
    {
        // This would need to be implemented with actual permission checking logic
        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets the current user's access rights for the specified type.
    /// </summary>
    /// <typeparam name="TAccessRights">The type of access rights to get.</typeparam>
    /// <returns>The access rights for the current user.</returns>
    public Task<TAccessRights> GetAccessRightsAsync<TAccessRights>() where TAccessRights : class, new()
    {
        // This would need to be implemented with actual access rights logic
        return Task.FromResult(new TAccessRights());
    }
} 