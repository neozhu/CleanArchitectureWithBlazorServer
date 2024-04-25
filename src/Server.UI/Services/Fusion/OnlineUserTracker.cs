using ActualLab.Fusion;
using ActualLab.Fusion.EntityFramework;
using ActualLab.Fusion.Extensions;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class OnlineUserTracker(IKeyValueStore store) : IOnlineUserTracker
{
    private DbShard _shard = DbShard.None;
    public async Task AddUser(string userId)
    {
        if (Computed.IsInvalidating())
            _ = GetOnlineUsers();

        await store.Set(_shard, userId, DateTime.UtcNow.ToString());

    }

    public Task<string[]> GetOnlineUsers()
    {
        if (Computed.IsInvalidating())
            return default!;
        return store.ListKeySuffixes(_shard, "", PageRef.New<string>(int.MaxValue));
    }

    public async Task RemoveUser(string userId)
    {
        if (Computed.IsInvalidating())
            _ = GetOnlineUsers();

        await store.Remove(_shard, userId);
    }
}
