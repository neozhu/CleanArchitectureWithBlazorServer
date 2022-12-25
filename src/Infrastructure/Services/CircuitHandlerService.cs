using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
public class CircuitHandlerService : CircuitHandler
{

    private readonly IUsersStateContainer _usersStateContainer;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CircuitHandlerService(IUsersStateContainer usersStateContainer, AuthenticationStateProvider authenticationStateProvider)
    {
        _usersStateContainer = usersStateContainer;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public override async Task OnConnectionUpAsync(Circuit circuit,
        CancellationToken cancellationToken)
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (state is not null && state.User.IsAuthenticated() && state.User.Identity is not null)
        {
            _usersStateContainer.Update(circuit.Id, state.User.Identity.Name);
        }
    }

    public override Task OnConnectionDownAsync(Circuit circuit,
        CancellationToken cancellationToken)
    {
        _usersStateContainer.Remove(circuit.Id);
        return Task.CompletedTask;
    }

}