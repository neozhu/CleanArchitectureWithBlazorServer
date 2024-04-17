
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Fluxor;
using Stl.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class UserSessionTracker : IUserSessionTracker
{

    private readonly ConcurrentDictionary<string, ImmutableHashSet<string>> _pageUserSessions;
    private readonly IMutableState<string> _state;
    public UserSessionTracker(IStateFactory stateFactory)
    {
        _state = stateFactory.NewMutable("");
        _pageUserSessions = new ConcurrentDictionary<string, ImmutableHashSet<string>>();
    }

    public async Task AddUser(string pagecomponent, string userName, CancellationToken cancellationToken = default)
    {
        _pageUserSessions.AddOrUpdate(pagecomponent,
             addValueFactory: _ => ImmutableHashSet.Create(userName),
             updateValueFactory: (_, existingSet) =>
                 existingSet.Add(userName)
         );
        var result = "";
        var users = _pageUserSessions.GetValueOrDefault(pagecomponent);
        if (users is not null)
        {
            result = string.Join(",", users.ToArray());
        }
        else
        {
            result = string.Empty;
        }
        _state.Value = result;
        await GetActiveUsers( cancellationToken);
    }

    public virtual async Task<string> GetActiveUsers( CancellationToken cancellationToken = default)
    {
        
        return await _state.Use(cancellationToken);
        //return Task.FromResult(result);
    }

    public async Task RemoveUser(string pagecomponent, string userName, CancellationToken cancellationToken = default)
    {
        _pageUserSessions.AddOrUpdate(pagecomponent,
             addValueFactory: _ => ImmutableHashSet<string>.Empty,
             updateValueFactory: (_, existingSet) =>
                 existingSet.Remove(userName)
         );
        var result = "";
        var users = _pageUserSessions.GetValueOrDefault(pagecomponent);
        if (users is not null)
        {
            result = string.Join(",", users.ToArray());
        }
        else
        {
            result = string.Empty;
        }
        _state.Value = result;
        await GetActiveUsers( cancellationToken);

    }
}
