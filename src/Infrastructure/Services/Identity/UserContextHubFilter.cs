using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// SignalR hub filter that sets user context when connections are established.
/// </summary>
public class UserContextHubFilter : IHubFilter
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUserContextAccessor _userContextAccessor;

    private const string Key = "__user_ctx";

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextHubFilter"/> class.
    /// </summary>
    /// <param name="userContextAccessor">The user context accessor.</param>
    /// <param name="userContextLoader">The user context loader.</param>
    public UserContextHubFilter(IServiceScopeFactory scopeFactory,IUserContextAccessor userContextAccessor)
    {
        _scopeFactory = scopeFactory;
        _userContextAccessor = userContextAccessor;
  
    }

    /// <summary>
    /// Invokes the hub method with user context set.
    /// </summary>
    /// <param name="invocationContext">The invocation context.</param>
    /// <param name="next">The next filter in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        invocationContext.Context.Items.TryGetValue(Key, out var val);
        var user = val as UserContext;

        using (_userContextAccessor.Push(user))
        {
            return await next(invocationContext);
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
        var principal = context.Context.User;
        if (principal?.Identity?.IsAuthenticated == true)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var loader = scope.ServiceProvider.GetRequiredService<IUserContextLoader>();
            var userContext = await loader.LoadAsync(principal, context.Context.ConnectionAborted);
            context.Context.Items[Key] = userContext;
        }

        await next(context);
    }

     
} 