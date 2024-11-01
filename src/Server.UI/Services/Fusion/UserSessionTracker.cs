using System.Collections.Immutable;
using System.Net.Http;
using ActualLab.Fusion;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class UserSessionTracker : IUserSessionTracker
{
    public UserSessionTracker(IHttpContextAccessor  httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private volatile ImmutableDictionary<string, ImmutableHashSet<SessionInfo>> _pageUserSessions = ImmutableDictionary<string, ImmutableHashSet<SessionInfo>>.Empty;
    private readonly IHttpContextAccessor _httpContextAccessor;

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

    public virtual  Task<List<SessionInfo>> GetUserSessions(string pageComponent,CancellationToken cancellationToken = default)
    {
        if (_pageUserSessions.TryGetValue(pageComponent, out var sessions))
        {
            return Task.FromResult(sessions.ToList());
        }

        return Task.FromResult(new List<SessionInfo>());
    }

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

    public virtual  Task RemoveAllSessions(string userId, CancellationToken cancellationToken = default)
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
    private  Task<SessionInfo> GetSessionInfo()
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
        return Task.FromResult(new SessionInfo(userId, userName, displayName, ipAddress,tenantId,"", UserPresence.Available));
    }

}