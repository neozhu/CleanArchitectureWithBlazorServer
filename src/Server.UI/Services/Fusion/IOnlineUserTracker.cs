using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public interface IOnlineUserTracker:IComputeService
{
    Task AddUser(Session session, string userId);
    Task RemoveUser(Session session, string userId);
    Task<List<string>> GetOnlineUsers(Session session);
}
