using System.Collections.Immutable;
using ActualLab.Fusion;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;


namespace CleanArchitecture.Blazor.Application.Features.Fusion;

/// <summary>
/// Tracks user sessions for different page components.
/// </summary>
public class UserSessionTracker : IUserSessionTracker
{
    private volatile ImmutableDictionary<string, ImmutableHashSet<SessionInfo>> _pageUserSessions = ImmutableDictionary<string, ImmutableHashSet<SessionInfo>>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionTracker"/> class.
    /// </summary>
    public UserSessionTracker()
    {

    }

    /// <summary>
    /// Adds a user session for the specified page component.
    /// </summary>
    /// <param name="pageComponent">The page component.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task AddUserSession(string pageComponent,SessionInfo sessionInfo, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;

  
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
    public virtual async Task RemoveUserSession(string pageComponent, string userId, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;

        if (_pageUserSessions.TryGetValue(pageComponent, out var users) && users.Any(x => x.UserId == userId))
        {
            var sessionInfo = users.First(x => x.UserId == userId);
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
            using var invalidating = Invalidation.Begin();
            _ = await GetUserSessions(pageComponent, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes all sessions for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task RemoveAllSessions(string userId, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;

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
            using var invalidating = Invalidation.Begin();
                _ = await GetUserSessions(pageComponent, cancellationToken).ConfigureAwait(false);
        }
        
    }

     
}
