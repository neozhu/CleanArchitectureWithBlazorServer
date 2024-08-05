using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class OnlineUserTracker : IOnlineUserTracker
{

    private readonly ConcurrentDictionary<string, UserInfo> _store = new();
    public async Task Add(string sessionId, UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        if (_store.TryAdd(sessionId, userInfo))
        {
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken);
        }
    }
    public async Task Update(string userId, string userName, string displayName, string profilePictureDataUrl, CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return;
        var invalidate = false;
        foreach (var key in _store.Keys)
        {
            if (_store[key].Name == userName)
            {
                var userInfo = _store[key] with
                {
                    Id = userId,
                    DisplayName = displayName,
                    ProfilePictureDataUrl = profilePictureDataUrl
                };
                var updated = _store.TryUpdate(key, userInfo, _store[key]);
                if (invalidate == false)
                {
                    invalidate = updated;
                }
            }
        }
        if (invalidate)
        {
            using var invalidating = Invalidation.Begin();
            _ = await GetOnlineUsers(cancellationToken);
        }
       

    }
    public virtual Task<UserInfo[]> GetOnlineUsers(CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
            return default!;
        return Task.FromResult(_store.Select(x => x.Value).Distinct(new UserInfoEqualityComparer()).ToArray());
    }

    public async Task Remove(string sessionId, CancellationToken cancellationToken = default)
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
public class UserInfoEqualityComparer : EqualityComparer<UserInfo?>
{
    public override bool Equals(UserInfo? x, UserInfo? y)
    {
        // Check whether the compared objects reference the same data.
        if (ReferenceEquals(x, y)) return true;

        // Check whether any of the compared objects is null.
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            return false;

        // Check whether the UserInfo properties are equal.
        return x.Name == y.Name; // Assuming Id is a relevant property for equality
    }

    public override int GetHashCode(UserInfo? obj)
    {
        // Check whether the object is null
        if (ReferenceEquals(obj, null)) return 0;

        // Get hash code for the Id field if it is not null.
        return obj.Id == null ? 0 : obj.Id.GetHashCode();
    }
}
