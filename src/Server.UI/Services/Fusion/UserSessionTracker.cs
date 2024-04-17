
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Stl.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Fusion;

public class UserSessionTracker : IUserSessionTracker
{
    private readonly IMutableState<string> _state;
    private readonly ConcurrentDictionary<string, ImmutableHashSet<string>> _pageUserSessions = new();

    public UserSessionTracker(IStateFactory stateFactory)
    {
        _state = stateFactory.NewMutable(string.Empty);
        _pageUserSessions = new ConcurrentDictionary<string, ImmutableHashSet<string>>();
    }

    public async Task AddUser(string pagecomponent, string userName, CancellationToken cancellationToken = default)
    {
        _pageUserSessions.AddOrUpdate(pagecomponent,
             addValueFactory: _ => ImmutableHashSet.Create(userName),
             updateValueFactory: (_, existingSet) =>
                 existingSet.Add(userName)
         );
        await GetActiveUsers(pagecomponent, userName, cancellationToken);
    }

    public virtual async Task<string> GetActiveUsers(string pagecomponent, string userName, CancellationToken cancellationToken = default)
    {
        var users = _pageUserSessions.GetValueOrDefault(pagecomponent);
        if (users is not null)
        {
            _state.Value = string.Join(", ", users.Where(x => !x.Equals(userName)));
        }
        else
        {
            _state.Value = string.Empty;
        }
        return await _state.Use(cancellationToken);
    }


    public async Task RemoveUser(string pagecomponent, string userName, CancellationToken cancellationToken = default)
    {
        _pageUserSessions.AddOrUpdate(pagecomponent,
             addValueFactory: _ => ImmutableHashSet<string>.Empty,
             updateValueFactory: (_, existingSet) =>
                 existingSet.Remove(userName)
         );

        await GetActiveUsers(pagecomponent, userName, cancellationToken);

    }
}
