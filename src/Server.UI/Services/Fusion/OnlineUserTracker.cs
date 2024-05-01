﻿using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class OnlineUserTracker : IOnlineUserTracker
{

    private readonly ConcurrentDictionary<string, UserInfo> _store = new();
    public async Task AddUser(string sessionId,UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        if (_store.TryAdd(sessionId, userInfo))
        {
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken);
        }
    }

    public virtual Task<UserInfo[]> GetOnlineUsers(CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return default!;
        return Task.FromResult(_store.Select(x=>x.Value).Distinct().ToArray());
    }

    public async Task RemoveUser(string sessionId, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        var removed = _store.TryRemove(sessionId, out var userInfo);
        if (removed)
        {
            using var invalidating = Invalidation.Begin();
            await GetOnlineUsers(cancellationToken);
        }
    }
}
