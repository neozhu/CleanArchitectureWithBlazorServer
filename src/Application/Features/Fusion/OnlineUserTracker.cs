using System.Collections.Immutable;
using ActualLab.Fusion;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Fusion;

/// <summary>
/// Tracks online users and manages their sessions.
/// </summary>
public class OnlineUserTracker : IOnlineUserTracker
{
    private volatile ImmutableHashSet<UserContext> _activeUserSessions = ImmutableHashSet<UserContext>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineUserTracker"/> class.
    /// </summary>
    public OnlineUserTracker()
    {
    }

    /// <summary>
    /// Initializes the session for a user.
    /// </summary>
    /// <param name="userContext">The user context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task Initialize(UserContext? userContext, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        if (userContext != null && !_activeUserSessions.Any(x => x.UserId == userContext.UserId))
        {
            _activeUserSessions = _activeUserSessions.Add(userContext);
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Clears all sessions for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task Clear(string userId, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        var userSessions = _activeUserSessions.Where(s => s.UserId == userId).ToList();
        foreach (var session in userSessions)
        {
            _activeUserSessions = _activeUserSessions.Remove(session);
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Updates the session information for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="profilePictureDataUrl">The profile picture data URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task Update(string userId, string userName, string displayName, string profilePictureDataUrl, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        var userSessions = _activeUserSessions.Where(s => s.UserId == userId).ToList();
        foreach (var session in userSessions)
        {
            //var updatedSession = new UserContext(userId, userName, session.TenantId, session.Email, session.Roles, session.SuperiorId);
            //_activeUserSessions = _activeUserSessions.Remove(session).Add(updatedSession);
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the list of online users.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of online user contexts.</returns>
    public virtual Task<List<UserContext>> GetOnlineUsers(CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return Task.FromResult(new List<UserContext>());

        return Task.FromResult(_activeUserSessions.ToList());
    }
}
