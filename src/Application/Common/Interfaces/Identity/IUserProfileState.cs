namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface for managing user profile state with immutable snapshots and precise notifications.
/// </summary>
public interface IUserProfileState
{
    /// <summary>
    /// Gets the current user profile snapshot (immutable).
    /// </summary>
    UserProfile Value { get; }

    /// <summary>
    /// Event triggered when the user profile changes.
    /// Subscribers receive the new UserProfile snapshot.
    /// </summary>
    event EventHandler<UserProfile>? Changed;

    /// <summary>
    /// Ensures the user profile is initialized for the given user ID.
    /// Only loads from database on first call or when user changes.
    /// </summary>
    /// <param name="userId">The user ID to initialize for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EnsureInitializedAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the user profile by clearing cache and reloading from database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a new user profile directly (for local updates after database changes).
    /// </summary>
    /// <param name="userProfile">The new user profile to set.</param>
    void Set(UserProfile userProfile);

    /// <summary>
    /// Updates user profile fields both in database and local state using a pure function.
    /// This is the recommended way to persist profile changes.
    /// </summary>
    /// <param name="change">Function that returns the updated profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <example>
    /// await userProfileState.UpdateAsync(profile => profile with
    /// {
    ///     DisplayName = "John Doe",
    ///     LanguageCode = "en-US"
    /// });
    /// </example>
    Task UpdateAsync(Func<UserProfile, UserProfile> change, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates profile fields locally (in-memory only) using a pure function.
    /// Use this for temporary updates that don't need to be persisted to the database.
    /// </summary>
    /// <param name="change">Function that returns the updated profile.</param>
    /// <example>
    /// userProfileState.UpdateLocal(profile => profile with
    /// {
    ///     DisplayName = "John Doe",
    ///     LanguageCode = "en-US"
    /// });
    /// </example>
    void UpdateLocal(Func<UserProfile, UserProfile> change);

   

    /// <summary>
    /// Clears the cache for the current user.
    /// </summary>
    void ClearCache();
}
