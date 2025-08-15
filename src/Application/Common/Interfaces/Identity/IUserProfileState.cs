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
    /// Updates the user's language code and persists it to the database.
    /// Also updates the local state and notifies subscribers.
    /// </summary>
    /// <param name="languageCode">The BCP-47 language tag (e.g., "en-US").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetLanguageAsync(string languageCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates specific fields locally without database access.
    /// </summary>
    /// <param name="profilePictureDataUrl">Profile picture data URL.</param>
    /// <param name="displayName">Display name.</param>
    /// <param name="phoneNumber">Phone number.</param>
    /// <param name="timeZoneId">Time zone ID.</param>
    /// <param name="languageCode">Language code.</param>
    void UpdateLocal(
        string? profilePictureDataUrl = null,
        string? displayName = null,
        string? phoneNumber = null,
        string? timeZoneId = null,
        string? languageCode = null);

    /// <summary>
    /// Clears the cache for the current user.
    /// </summary>
    void ClearCache();
}
