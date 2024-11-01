using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using ActualLab.Fusion;


namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class OnlineUserTracker : IOnlineUserTracker
{
    public OnlineUserTracker(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private volatile ImmutableHashSet<SessionInfo> _activeUserSessions =  ImmutableHashSet<SessionInfo>.Empty;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public virtual async Task Initial( SessionInfo sessionInfo, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        if (!_activeUserSessions.Any(x=>x.UserId==sessionInfo.UserId))
        {
            _activeUserSessions = _activeUserSessions.Add(sessionInfo);
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

    public virtual async Task Logout(CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        var sessionInfo = await GetSessionInfo().ConfigureAwait(false);
        var userSessions = _activeUserSessions.Where(s => s.UserId == sessionInfo.UserId).ToList();
        foreach (var session in userSessions)
        {
            _activeUserSessions = _activeUserSessions.Remove(session);
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken).ConfigureAwait(false);
        }
    }

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

    public virtual Task<List<SessionInfo>> GetOnlineUsers(CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return Task.FromResult(new List<SessionInfo>());

        return Task.FromResult(_activeUserSessions.ToList());
    }



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
