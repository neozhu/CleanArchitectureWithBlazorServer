using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;
public class UsersStateContainer : IUsersStateContainer
{
    public ConcurrentDictionary<string, string> UsersByConnectionId { get; } = new ConcurrentDictionary<string, string>();

    public event Action? OnChange;
    public void Update(string connectionId, string? name)
    {
        UsersByConnectionId.AddOrUpdate(connectionId, name??String.Empty, (key, oldValue) => name??String.Empty);
        NotifyStateChanged();
    }
    public void Remove(string connectionId)
    {
        UsersByConnectionId.TryRemove(connectionId, out var _);
        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnChange?.Invoke();
}
