using System.Collections.Concurrent;

namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Interfaces.Identity;
public interface IUsersStateContainer
{
    ConcurrentDictionary<string, string> UsersByConnectionId { get; }
    event Action? OnChange;
    void Update(string connectionId, string? name);
    void Remove(string connectionId);
}
