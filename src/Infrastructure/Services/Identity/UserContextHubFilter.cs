using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// SignalR hub filter that sets user context when connections are established.
/// </summary>
public class UserContextHubFilter : IHubFilter
{
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUserContextLoader _userContextLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextHubFilter"/> class.
    /// </summary>
    /// <param name="userContextAccessor">The user context accessor.</param>
    /// <param name="userContextLoader">The user context loader.</param>
    public UserContextHubFilter(IUserContextAccessor userContextAccessor, IUserContextLoader userContextLoader)
    {
        _userContextAccessor = userContextAccessor;
        _userContextLoader = userContextLoader;
    }

    /// <summary>
    /// Invokes the hub method with user context set.
    /// </summary>
    /// <param name="invocationContext">The invocation context.</param>
    /// <param name="next">The next filter in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var user = invocationContext.Context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            await _userContextLoader.LoadAndSetAsync(user, _userContextAccessor);
        }

        try
        {
            return await next(invocationContext);
        }
        finally
        {
            // Clear the context after the method invocation
            _userContextAccessor.Clear();
        }
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    /// <param name="context">The hub context.</param>
    /// <param name="next">The next filter in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        var user = context.Context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            await _userContextLoader.LoadAndSetAsync(user, _userContextAccessor);
        }

        try
        {
            await next(context);
        }
        finally
        {
            // Clear the context when the connection is closed
            _userContextAccessor.Clear();
        }
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    /// <param name="context">The hub context.</param>
    /// <param name="exception">The exception that caused the disconnection, if any.</param>
    /// <param name="next">The next filter in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
    {
        try
        {
            await next(context, exception);
        }
        finally
        {
            // Clear the context when the connection is closed
            _userContextAccessor.Clear();
        }
    }
} 