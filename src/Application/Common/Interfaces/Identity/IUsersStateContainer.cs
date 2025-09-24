using System.Collections.Concurrent;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

public interface IUsersStateContainer
{
    ConcurrentDictionary<string, string> UsersByConnectionId { get; }
    event Action? OnChange;
    void AddOrUpdate(string connectionId, string? userId);
    void Remove(string connectionId);
    void Clear(string userId);
}
