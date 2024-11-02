using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using ActualLab.Fusion;


namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

/// <summary>
/// Tracks online users and manages their sessions.
/// </summary>
public class OnlineUserTracker : IOnlineUserTracker
{
    private volatile ImmutableHashSet<SessionInfo> _activeUserSessions = ImmutableHashSet<SessionInfo>.Empty;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineUserTracker"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public OnlineUserTracker(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Initializes the session for a user.
    /// </summary>
    /// <param name="sessionInfo">The session information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task Initial(SessionInfo sessionInfo, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        if (!_activeUserSessions.Any(x => x.UserId == sessionInfo.UserId))
        {
            _activeUserSessions = _activeUserSessions.Add(sessionInfo);
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
            var updatedSession = new SessionInfo(userId, userName, displayName, session.IPAddress, session.TenantId, profilePictureDataUrl, session.Status);
            _activeUserSessions = _activeUserSessions.Remove(session).Add(updatedSession);
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the list of online users.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of online user sessions.</returns>
    public virtual Task<List<SessionInfo>> GetOnlineUsers(CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return Task.FromResult(new List<SessionInfo>());

        return Task.FromResult(_activeUserSessions.ToList());
    }
}
