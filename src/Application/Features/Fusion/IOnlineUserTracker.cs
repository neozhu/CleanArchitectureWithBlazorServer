using ActualLab.Fusion;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Fusion;

public interface IOnlineUserTracker : IComputeService
{
    Task Initialize(UserContext? userContext,CancellationToken cancellationToken = default);
    Task Clear(string userId,CancellationToken cancellationToken = default);
    Task Update(string userId,string userName,string displayName,string profilePictureDataUrl, CancellationToken cancellationToken = default);
    [ComputeMethod]
    Task<List<UserContext>> GetOnlineUsers(CancellationToken cancellationToken = default);
}

 