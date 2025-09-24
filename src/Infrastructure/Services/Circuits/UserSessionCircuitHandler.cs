using CleanArchitecture.Blazor.Application.Common.Extensions;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Circuits;

/// <summary>
/// Handles user session tracking and online user tracking for Blazor server circuits.
/// </summary>
public class UserSessionCircuitHandler : CircuitHandler
{

    private readonly IUsersStateContainer _usersStateContainer;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionCircuitHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="userContextAccessor">The user context accessor.</param>
    /// <param name="userContextLoader">The user context loader.</param>
    /// <param name="authenticationStateProvider">The authentication state provider.</param>
    public UserSessionCircuitHandler(
        IUsersStateContainer usersStateContainer,
        AuthenticationStateProvider authenticationStateProvider)
    {
  
        _usersStateContainer = usersStateContainer;
        _authenticationStateProvider = authenticationStateProvider;
    }




    /// <summary>
    /// Called when a new circuit connection is established.
    /// </summary>
    /// <param name="circuit">The circuit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();  
        if (state.User.Identity?.IsAuthenticated == true)
        {
            var userId = state.User.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                _usersStateContainer.AddOrUpdate(circuit.Id, userId);
            }
        }
        
        await base.OnConnectionUpAsync(circuit, cancellationToken);
    }

    /// <summary>
    /// Called when a circuit connection is disconnected.
    /// </summary>
    /// <param name="circuit">The circuit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _usersStateContainer.Remove(circuit.Id);
        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }

    
}
