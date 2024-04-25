using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public interface IOnlineUserTracker:IComputeService
{
    Task AddUser(string userId, CancellationToken cancellationToken = default);
    Task RemoveUser(string userId, CancellationToken cancellationToken = default);
    Task<string[]> GetOnlineUsers( CancellationToken cancellationToken=default);
}
