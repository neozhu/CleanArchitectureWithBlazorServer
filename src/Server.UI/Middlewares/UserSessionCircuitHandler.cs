using CleanArchitecture.Blazor.Server.UI.Services.Fusion;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Server.UI.Middlewares;

public class UserSessionCircuitHandler : CircuitHandler
{
    private readonly IUserSessionTracker _userSessionTracker;
    private readonly IOnlineUserTracker _onlineUserTracker;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public UserSessionCircuitHandler(IUserSessionTracker userSessionTracker, IOnlineUserTracker OnlineUserTracker, IHttpContextAccessor httpContextAccessor)
    {
        _userSessionTracker = userSessionTracker;
        _onlineUserTracker = OnlineUserTracker;
        _httpContextAccessor = httpContextAccessor;
  
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await base.OnConnectionUpAsync(circuit, cancellationToken);
    }

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
