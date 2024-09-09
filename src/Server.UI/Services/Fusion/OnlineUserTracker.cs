using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class OnlineUserTracker : IOnlineUserTracker
{
    // A concurrent dictionary to store user information by session ID.
    private readonly ConcurrentDictionary<string, UserInfo> _store = new();

    /// <summary>
    /// Adds a user session to the tracker.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="userInfo">The user information to be added.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public async Task Add(string sessionId, UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        // If the invalidation is active, skip adding.
        if (Invalidation.IsActive)
            return;

        // Try to add the user information into the store.
        if (_store.TryAdd(sessionId, userInfo))
        {
            using var invalidating = Invalidation.Begin();
            // Get the updated online users list asynchronously.
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Updates a user's information in the tracker.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="userName">The user's name.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="profilePictureDataUrl">The profile picture URL.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public async Task Update(string userId, string userName, string displayName, string profilePictureDataUrl, CancellationToken cancellationToken = default)
    {
        // If invalidation is active, skip updating.
        if (Invalidation.IsActive)
            return;

        var invalidate = false;

        // Loop through the store's keys and update user information based on user name.
        foreach (var key in _store.Keys)
        {
            // Use string.Equals with explicit comparison to avoid using '=='
            if (string.Equals(_store[key].Name, userName, StringComparison.Ordinal))
            {
                // Update the user information.
                var userInfo = _store[key] with
                {
                    Id = userId,
                    DisplayName = displayName,
                    ProfilePictureDataUrl = profilePictureDataUrl
                };

                // Try to update the entry in the dictionary.
                var updated = _store.TryUpdate(key, userInfo, _store[key]);
                if (!invalidate)
                {
                    invalidate = updated;
                }
            }
        }

        // If any user information was updated, invalidate and get the updated list.
        if (invalidate)
        {
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Retrieves the list of online users.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task containing the array of online users.</returns>
    public virtual Task<UserInfo[]> GetOnlineUsers(CancellationToken cancellationToken = default)
    {
        // Return an empty array if invalidation is active to avoid null return values.
        if (Invalidation.IsActive)
            return Task.FromResult(Array.Empty<UserInfo>()); // Avoid returning null

        // Return the distinct list of online users.
        return Task.FromResult(_store.Select(x => x.Value).Distinct(new UserInfoEqualityComparer()).ToArray());
    }

    /// <summary>
    /// Removes a user session from the tracker.
    /// </summary>
    /// <param name="sessionId">The session ID to remove.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public async Task Remove(string sessionId, CancellationToken cancellationToken = default)
    {
        // If invalidation is active, skip removal.
        if (Invalidation.IsActive)
            return;

        // Try to remove the session from the store.
        var removed = _store.TryRemove(sessionId, out var userInfo);

        // If removed, invalidate and update the list.
        if (removed)
        {
            using var invalidating = Invalidation.Begin();
            await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }
}

public class UserInfoEqualityComparer : EqualityComparer<UserInfo?>
{
    /// <summary>
    /// Compares two UserInfo objects for equality based on their name.
    /// </summary>
    /// <param name="x">First UserInfo object.</param>
    /// <param name="y">Second UserInfo object.</param>
    /// <returns>True if the names are equal, otherwise false.</returns>
    public override bool Equals(UserInfo? x, UserInfo? y)
    {
        // Check if both references point to the same object.
        if (ReferenceEquals(x, y)) return true;

        // Return false if either object is null.
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            return false;

        // Compare the names of the UserInfo objects.
        return string.Equals(x.Name, y.Name, StringComparison.Ordinal); // Use string.Equals for better control.
    }

    /// <summary>
    /// Returns the hash code for a UserInfo object based on the Id.
    /// </summary>
    /// <param name="obj">The UserInfo object.</param>
    /// <returns>The hash code based on the user's Id.</returns>
    public override int GetHashCode(UserInfo? obj)
    {
        // Return 0 if the object is null.
        if (ReferenceEquals(obj, null)) return 0;

        // Use StringComparer to compute the hash code of the Id.
        return StringComparer.Ordinal.GetHashCode(obj.Id ?? string.Empty);
    }
}
