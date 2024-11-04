using CleanArchitecture.Blazor.Application.Features.Fusion;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Circuits;

/// <summary>
/// Handles user session tracking and online user tracking for Blazor server circuits.
/// </summary>
public class UserSessionCircuitHandler : CircuitHandler
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionCircuitHandler"/> class.
    /// </summary>
    /// <param name="userSessionTracker">The user session tracker service.</param>
    /// <param name="onlineUserTracker">The online user tracker service.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public UserSessionCircuitHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Called when a new circuit connection is established.
    /// </summary>
    /// <param name="circuit">The circuit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var usersStateContainer = _serviceProvider.GetRequiredService<IUsersStateContainer>();
        var currentUserContextSetter = _serviceProvider.GetRequiredService<ICurrentUserContextSetter>();
        var context = httpContextAccessor.HttpContext;

        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            var sessionInfo = new SessionInfo
            (
                context.User.GetUserId(),
                context.User.GetUserName(),
                context.User.GetDisplayName(),
                context.Connection.RemoteIpAddress?.ToString(),
                context.User.GetTenantId(),
                context.User.GetProfilePictureDataUrl(),
                UserPresence.Available
            );
            currentUserContextSetter.SetCurrentUser(sessionInfo);
            if (!string.IsNullOrEmpty(sessionInfo.UserId))
            {
                usersStateContainer.AddOrUpdate(circuit.Id, sessionInfo.UserId);
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
        var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var userSessionTracker = _serviceProvider.GetRequiredService<IUserSessionTracker>();
        var onlineUserTracker = _serviceProvider.GetRequiredService<IOnlineUserTracker>();
        var usersStateContainer = _serviceProvider.GetRequiredService<IUsersStateContainer>();
        var currentUserContextSetter = _serviceProvider.GetRequiredService<ICurrentUserContextSetter>();
        var userId = (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false) ? httpContextAccessor.HttpContext?.User.GetUserId() : string.Empty;
        if (!string.IsNullOrEmpty(userId))
        {
            await userSessionTracker.RemoveAllSessions(userId, cancellationToken);
            await onlineUserTracker.Clear(userId, cancellationToken);
            usersStateContainer.Remove(circuit.Id);
        }

        currentUserContextSetter.Clear();
        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }
}
