namespace CleanArchitecture.Blazor.Application.Common.Constants;

/// <summary>
/// Provides constants for user-related cache keys.
/// Centralizes cache key management to avoid duplication and ensure consistency.
/// </summary>
public static class UserCacheKeys
{
    private const string UserPrefix = "User";

    /// <summary>
    /// Gets the cache key for user context.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The cache key for user context.</returns>
    public static string UserContext(string userId) => $"{UserPrefix}:Context:{userId}";

    /// <summary>
    /// Gets the cache key for user profile.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The cache key for user profile.</returns>
    public static string UserProfile(string userId) => $"{UserPrefix}:Profile:{userId}";

    /// <summary>
    /// Gets the cache key for user application.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The cache key for user application.</returns>
    public static string UserApplication(string userId) => $"{UserPrefix}:Application:{userId}";

    /// <summary>
    /// Gets the cache key for user claims.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The cache key for user claims.</returns>
    public static string UserClaims(string userId) => $"{UserPrefix}:Claims:{userId}";

    /// <summary>
    /// Gets the cache key for user roles.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The cache key for user roles.</returns>
    public static string UserRoles(string userId) => $"{UserPrefix}:Roles:{userId}";

    /// <summary>
    /// Gets the cache key for user permissions.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The cache key for user permissions.</returns>
    public static string UserPermissions(string userId) => $"{UserPrefix}:Permissions:{userId}";

    /// <summary>
    /// Gets the cache key for role claims.
    /// </summary>
    /// <param name="roleId">The role ID.</param>
    /// <returns>The cache key for role claims.</returns>
    public static string RoleClaims(string roleId) => $"Role:Claims:{roleId}";

    /// <summary>
    /// Gets all possible cache keys for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Array of all cache keys for the user.</returns>
    public static string[] AllUserKeys(string userId)
    {
        return
        [
            UserContext(userId),
            UserProfile(userId),
            UserApplication(userId),
            UserClaims(userId),
            UserRoles(userId),
            UserPermissions(userId)
        ];
    }

    /// <summary>
    /// Gets a cache key for a specific user and cache type.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cacheType">The cache type</param>
    /// <returns>The cache key</returns>
    public static string GetCacheKey(string userId, UserCacheType cacheType)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return string.Empty;

        return cacheType switch
        {
            UserCacheType.Context => UserContext(userId),
            UserCacheType.Profile => UserProfile(userId),
            UserCacheType.Application => UserApplication(userId),
            UserCacheType.Claims => UserClaims(userId),
            UserCacheType.Roles => UserRoles(userId),
            UserCacheType.Permissions => UserPermissions(userId),
            _ => string.Empty
        };
    }
}

/// <summary>
/// Types of user-related cache entries.
/// </summary>
public enum UserCacheType
{
    Context,
    Profile,
    Application,
    Claims,
    Roles,
    Permissions
}
