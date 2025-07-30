using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IUserContextAccessor using AsyncLocal for call chain isolation.
/// </summary>
public class UserContextAccessor : IUserContextAccessor
{
    private static readonly AsyncLocal<Stack<UserContext>> _contextStack = new();
    private readonly ILogger<UserContextAccessor> _logger;

    public UserContextAccessor(ILogger<UserContextAccessor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user context.
    /// </summary>
    public UserContext? Current 
    { 
        get 
        { 
            var current = _contextStack.Value?.Count > 0 ? _contextStack.Value.Peek() : null;
            _logger.LogDebug("UserContextAccessor: Current user context is {UserName}", current?.UserName ?? "null");
            return current;
        }
    }

    /// <summary>
    /// Pushes a new user context onto the stack.
    /// </summary>
    /// <param name="context">The user context to push.</param>
    /// <returns>A disposable object that will pop the context when disposed.</returns>
    public IDisposable Push(UserContext context)
    {
        if (_contextStack.Value == null)
        {
            _contextStack.Value = new Stack<UserContext>();
        }

        _contextStack.Value.Push(context);
        return new UserContextScope(this);
    }

    /// <summary>
    /// Pops the current user context from the stack.
    /// </summary>
    public void Pop()
    {
        if (_contextStack.Value?.Count > 0)
        {
            _contextStack.Value.Pop();
        }
    }

    /// <summary>
    /// Sets the current user context.
    /// </summary>
    /// <param name="context">The user context to set.</param>
    public void Set(UserContext context)
    {
        _logger.LogInformation("UserContextAccessor: Setting user context for {UserName}", context.UserName);
        if (_contextStack.Value == null)
        {
            _contextStack.Value = new Stack<UserContext>();
        }
        else
        {
            _contextStack.Value.Clear();
        }

        _contextStack.Value.Push(context);
    }

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    public void Clear()
    {
        _logger.LogInformation("UserContextAccessor: Clearing user context");
        _contextStack.Value?.Clear();
    }

    /// <summary>
    /// Disposable scope for managing user context lifecycle.
    /// </summary>
    private class UserContextScope : IDisposable
    {
        private readonly UserContextAccessor _accessor;

        public UserContextScope(UserContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public void Dispose()
        {
            _accessor.Pop();
        }
    }
} 