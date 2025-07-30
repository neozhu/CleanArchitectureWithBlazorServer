using CleanArchitecture.Blazor.Application.Features.Fusion;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Circuits;

/// <summary>
/// Handles user session tracking and online user tracking for Blazor server circuits.
/// </summary>
public class UserSessionCircuitHandler : CircuitHandler, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUserContextLoader _userContextLoader;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionCircuitHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="userContextAccessor">The user context accessor.</param>
    /// <param name="userContextLoader">The user context loader.</param>
    /// <param name="authenticationStateProvider">The authentication state provider.</param>
    public UserSessionCircuitHandler(
        IServiceProvider serviceProvider, 
        IUserContextAccessor userContextAccessor,
        IUserContextLoader userContextLoader,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _serviceProvider = serviceProvider;
        _userContextAccessor = userContextAccessor;
        _userContextLoader = userContextLoader;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _authenticationStateProvider.AuthenticationStateChanged += AuthenticationChanged;
        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    private void AuthenticationChanged(Task<AuthenticationState> task)
    {
        _ = UpdateAuthentication(task);

        async Task UpdateAuthentication(Task<AuthenticationState> task)
        {
            try
            {
                var state = await task;
                await _userContextLoader.LoadAndSetAsync(state.User, _userContextAccessor);
            }
            catch
            {
                // Ignore authentication errors
            }
        }
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
        await _userContextLoader.LoadAndSetAsync(state.User, _userContextAccessor);
        
        var usersStateContainer = _serviceProvider.GetRequiredService<IUsersStateContainer>();
        
        if (state.User.Identity?.IsAuthenticated == true)
        {
            var userId = state.User.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                usersStateContainer.AddOrUpdate(circuit.Id, userId);
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
        var userSessionTracker = _serviceProvider.GetRequiredService<IUserSessionTracker>();
        var onlineUserTracker = _serviceProvider.GetRequiredService<IOnlineUserTracker>();
        var usersStateContainer = _serviceProvider.GetRequiredService<IUsersStateContainer>();
        
        var currentUser = _userContextAccessor.Current;
        if (currentUser != null)
        {
            await userSessionTracker.RemoveAllSessions(currentUser.UserId, cancellationToken);
            await onlineUserTracker.Clear(currentUser.UserId, cancellationToken);
            usersStateContainer.Remove(circuit.Id);
        }

        _userContextAccessor.Clear();
        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }

    public void Dispose()
    {
        _authenticationStateProvider.AuthenticationStateChanged -= AuthenticationChanged;
    }
}