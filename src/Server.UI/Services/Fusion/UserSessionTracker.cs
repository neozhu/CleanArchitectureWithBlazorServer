using System.Collections.Immutable;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class UserSessionTracker : IUserSessionTracker
{
    private readonly object _lock = new object();
    private volatile ImmutableDictionary<string, ImmutableHashSet<string>> _pageUserSessions = ImmutableDictionary<string, ImmutableHashSet<string>>.Empty;

    public async Task AddUser(string pageComponent, string userName, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // 尝试获取指定页面的用户集合
            if (_pageUserSessions.TryGetValue(pageComponent, out var existingUsers))
            {
                // 如果用户名不存在于当前集合，更新集合
                if (!existingUsers.Contains(userName))
                {
                    var updatedUsers = existingUsers.Add(userName);
                    _pageUserSessions = _pageUserSessions.SetItem(pageComponent, updatedUsers);
                }
            }
            else
            {
                // 页面组件不存在，创建新的用户集合
                _pageUserSessions = _pageUserSessions.Add(pageComponent, ImmutableHashSet.Create(userName));
            }
        }

        // 异步获取最新的用户会话列表，这里不需要等待返回结果，因此使用 _ = 可以忽略返回的任务
        _ =await GetUserSessions(cancellationToken);
    }

    public virtual async Task<(string PageComponent, string[] UserSessions)[]> GetUserSessions(CancellationToken cancellationToken = default)
    {
        return _pageUserSessions.Select(kvp => (kvp.Key, kvp.Value.ToArray())).ToArray();
    }

    public async Task RemoveUser(string pageComponent, string userName, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_pageUserSessions.TryGetValue(pageComponent, out var users) && users.Contains(userName))
            {
                var updatedUsers = users.Remove(userName);
                if (updatedUsers.IsEmpty)
                {
                    _pageUserSessions = _pageUserSessions.Remove(pageComponent);
                }
                else
                {
                    _pageUserSessions = _pageUserSessions.SetItem(pageComponent, updatedUsers);
                }
            }
        }

        // 异步获取最新的用户会话列表，这里不需要等待完成，因此使用 _ = 可以忽略返回的任务
        _ = await GetUserSessions(cancellationToken);
    }
}