using ActualLab;
using System.Runtime.Serialization;
using ActualLab.Fusion;
using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public interface IOnlineUserTracker:IComputeService
{
    Task AddUser(string userId, CancellationToken cancellationToken = default);
    Task UpdateUser(string userId, CancellationToken cancellationToken = default);
    Task RemoveUser(string userId, CancellationToken cancellationToken = default);
    Task<Dictionary<string, UserProfile>> GetOnlineUsers( CancellationToken cancellationToken=default);
    Task<UserProfile> Get(string userId, CancellationToken cancellationToken = default);
}

 