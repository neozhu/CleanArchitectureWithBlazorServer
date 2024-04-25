using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public interface IOnlineUserTracker:IComputeService
{
    Task AddUser(string userId);
    Task RemoveUser(string userId);
    Task<string[]> GetOnlineUsers();
}
