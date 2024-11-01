﻿using CleanArchitecture.Blazor.Server.UI.Services.Fusion;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Server.UI.Middlewares;

/// <summary>
/// Handles user session tracking and online user tracking for Blazor server circuits.
/// </summary>
public class UserSessionCircuitHandler : CircuitHandler
{
    private readonly IUserSessionTracker _userSessionTracker;
    private readonly IOnlineUserTracker _onlineUserTracker;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionCircuitHandler"/> class.
    /// </summary>
    /// <param name="userSessionTracker">The user session tracker service.</param>
    /// <param name="onlineUserTracker">The online user tracker service.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public UserSessionCircuitHandler(IUserSessionTracker userSessionTracker, IOnlineUserTracker onlineUserTracker, IHttpContextAccessor httpContextAccessor)
    {
        _userSessionTracker = userSessionTracker;
        _onlineUserTracker = onlineUserTracker;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Called when a new circuit connection is established.
    /// </summary>
    /// <param name="circuit">The circuit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
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
        var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

        if (!string.IsNullOrEmpty(userId))
        {
            await _userSessionTracker.RemoveAllSessions(userId, cancellationToken);
            await _onlineUserTracker.Clear(userId, cancellationToken);
        }

        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }
}
