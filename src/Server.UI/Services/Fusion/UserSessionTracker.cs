using System.Collections.Immutable;
using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

/// <summary>
/// Tracks user sessions for different page components.
/// </summary>
public class UserSessionTracker : IUserSessionTracker
{
    private volatile ImmutableDictionary<string, ImmutableHashSet<SessionInfo>> _pageUserSessions = ImmutableDictionary<string, ImmutableHashSet<SessionInfo>>.Empty;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionTracker"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public UserSessionTracker(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Adds a user session for the specified page component.
    /// </summary>
    /// <param name="pageComponent">The page component.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task AddUserSession(string pageComponent, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;

        var sessionInfo = await GetSessionInfo().ConfigureAwait(false);
        ImmutableInterlocked.AddOrUpdate(
            ref _pageUserSessions,
            pageComponent,
            ImmutableHashSet.Create(sessionInfo),
            (key, existingSessions) => existingSessions.Add(sessionInfo));

        using var invalidating = Invalidation.Begin();
        _ = await GetUserSessions(pageComponent, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the user sessions for the specified page component.
    /// </summary>
    /// <param name="pageComponent">The page component.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of session information.</returns>
    public virtual Task<List<SessionInfo>> GetUserSessions(string pageComponent, CancellationToken cancellationToken = default)
    {
        if (_pageUserSessions.TryGetValue(pageComponent, out var sessions))
        {
            return Task.FromResult(sessions.ToList());
        }

        return Task.FromResult(new List<SessionInfo>());
    }

    /// <summary>
    /// Removes a user session for the specified page component.
    /// </summary>
    /// <param name="pageComponent">The page component.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task RemoveUserSession(string pageComponent, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;

        var sessionInfo = await GetSessionInfo().ConfigureAwait(false);

        if (_pageUserSessions.TryGetValue(pageComponent, out var users) && users.Contains(sessionInfo))
        {
            var updatedUsers = users.Remove(sessionInfo);

            // Use atomic update to prevent concurrency issues
            if (updatedUsers.IsEmpty)
            {
                ImmutableInterlocked.TryRemove(ref _pageUserSessions, pageComponent, out _);
            }
            else
            {
                ImmutableInterlocked.AddOrUpdate(
                    ref _pageUserSessions,
                    pageComponent,
                    updatedUsers,
                    (key, existingUsers) => updatedUsers);
            }
        }

        using var invalidating = Invalidation.Begin();
        _ = await GetUserSessions(pageComponent, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes all sessions for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual Task RemoveAllSessions(string userId, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return Task.CompletedTask;

        foreach (var pageComponent in _pageUserSessions.Keys.ToList())
        {
            if (_pageUserSessions.TryGetValue(pageComponent, out var users))
            {
                var updatedUsers = users.Where(user => user.UserId != userId).ToImmutableHashSet();

                // Use atomic update to prevent concurrency issues
                if (updatedUsers.IsEmpty)
                {
                    ImmutableInterlocked.TryRemove(ref _pageUserSessions, pageComponent, out _);
                }
                else
                {
                    ImmutableInterlocked.AddOrUpdate(
                        ref _pageUserSessions,
                        pageComponent,
                        updatedUsers,
                        (key, existingUsers) => updatedUsers);
                }
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the session information from the current HTTP context.
    /// </summary>
    /// <returns>The session information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the HTTP context is not available.</exception>
    private Task<SessionInfo> GetSessionInfo()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new InvalidOperationException("HttpContext is not available.");
        }
        var httpUser = _httpContextAccessor.HttpContext?.User;
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
        var userName = _httpContextAccessor.HttpContext?.User?.GetUserName();
        var displayName = _httpContextAccessor.HttpContext?.User?.GetDisplayName();
        var tenantId = _httpContextAccessor.HttpContext?.User?.GetTenantId();
        return Task.FromResult(new SessionInfo(userId, userName, displayName, ipAddress, tenantId, "", UserPresence.Available));
    }
}
