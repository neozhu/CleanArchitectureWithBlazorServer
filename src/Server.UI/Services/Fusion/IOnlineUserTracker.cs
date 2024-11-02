using ActualLab.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public interface IOnlineUserTracker : IComputeService
{
    Task Initial(SessionInfo sessionInfo,CancellationToken cancellationToken = default);
    Task Clear(string userId,CancellationToken cancellationToken = default);
    Task Update(string userId,string userName,string displayName,string profilePictureDataUrl, CancellationToken cancellationToken = default);
    [ComputeMethod]
    Task<List<SessionInfo>> GetOnlineUsers(CancellationToken cancellationToken = default);

}

 