using System.Collections.Concurrent;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;


/// <summary>
/// Manages the state of users by their connection IDs.
/// </summary>
public class UsersStateContainer : IUsersStateContainer
{
    /// <summary>
    /// Gets the dictionary that maps connection IDs to user names.
    /// </summary>
    public ConcurrentDictionary<string, string> UsersByConnectionId { get; } = new();

    /// <summary>
    /// Event triggered when the state changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Adds or updates a user in the state container.
    /// </summary>
    /// <param name="connectionId">The connection ID of the user.</param>
    /// <param name="name">The name of the user.</param>
    public void AddOrUpdate(string connectionId, string? name)
    {
        UsersByConnectionId.AddOrUpdate(connectionId, name ?? string.Empty, (key, oldValue) => name ?? string.Empty);
        NotifyStateChanged();
    }

    /// <summary>
    /// Removes a user from the state container by their connection ID.
    /// </summary>
    /// <param name="connectionId">The connection ID of the user to remove.</param>
    public void Remove(string connectionId)
    {
        UsersByConnectionId.TryRemove(connectionId, out _);
        NotifyStateChanged();
    }

    /// <summary>
    /// Clears all users with the specified name from the state container.
    /// </summary>
    /// <param name="userName">The name of the user to clear.</param>
    public void Clear(string userName)
    {
        var keysToRemove = UsersByConnectionId.Where(kvp => kvp.Value == userName).Select(kvp => kvp.Key).ToList();
        foreach (var key in keysToRemove)
        {
            UsersByConnectionId.TryRemove(key, out _);
        }
        NotifyStateChanged();
    }

    /// <summary>
    /// Notifies subscribers that the state has changed.
    /// </summary>
    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
