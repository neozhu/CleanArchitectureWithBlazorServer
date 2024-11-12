using CleanArchitecture.Blazor.Application.Features.Fusion;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Circuits;

/// <summary>
/// Handles user session tracking and online user tracking for Blazor server circuits.
/// </summary>
public class UserSessionCircuitHandler : CircuitHandler,IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICurrentUserContextSetter _currentUserContextSetter;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionCircuitHandler"/> class.
    /// </summary>
    /// <param name="userSessionTracker">The user session tracker service.</param>
    /// <param name="onlineUserTracker">The online user tracker service.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public UserSessionCircuitHandler(IServiceProvider serviceProvider, ICurrentUserContextSetter currentUserContextSetter, AuthenticationStateProvider authenticationStateProvider)
    {
        _serviceProvider = serviceProvider;
        _currentUserContextSetter = currentUserContextSetter;
        _authenticationStateProvider = authenticationStateProvider;
    }
    public override Task OnCircuitOpenedAsync(Circuit circuit,
        CancellationToken cancellationToken)
    {
        _authenticationStateProvider.AuthenticationStateChanged +=
            AuthenticationChanged;

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
                _currentUserContextSetter.SetCurrentUser(state.User);
            }
            catch
            {
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
        _currentUserContextSetter.SetCurrentUser(state.User);
        var usersStateContainer = _serviceProvider.GetRequiredService<IUsersStateContainer>();
        var currentUserContextSetter = _serviceProvider.GetRequiredService<ICurrentUserContextSetter>();
        if (state.User.Identity?.IsAuthenticated??false)
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
        var currentUserAccessor = _serviceProvider.GetRequiredService<ICurrentUserAccessor>();
        var userSessionTracker = _serviceProvider.GetRequiredService<IUserSessionTracker>();
        var onlineUserTracker = _serviceProvider.GetRequiredService<IOnlineUserTracker>();
        var usersStateContainer = _serviceProvider.GetRequiredService<IUsersStateContainer>();
        var currentUserContextSetter = _serviceProvider.GetRequiredService<ICurrentUserContextSetter>();
        if (currentUserAccessor.SessionInfo != null)
        {
            await userSessionTracker.RemoveAllSessions(currentUserAccessor.SessionInfo.UserId, cancellationToken);
            await onlineUserTracker.Clear(currentUserAccessor.SessionInfo.UserId, cancellationToken);
            usersStateContainer.Remove(circuit.Id);
        }

        currentUserContextSetter.Clear();
        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }

    public void Dispose()
    {
        _authenticationStateProvider.AuthenticationStateChanged -=
            AuthenticationChanged;
    }
}