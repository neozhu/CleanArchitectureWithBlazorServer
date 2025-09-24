namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IUserContextAccessor using AsyncLocal for call chain isolation.
/// </summary>
public class UserContextAccessor : IUserContextAccessor
{
    private sealed class Node
    {
        public UserContext? Value;
        public Node? Parent;
    }


    private readonly AsyncLocal<Node?> _current = new();
    /// <summary>
    /// Gets the current user context.
    /// </summary>
    public UserContext? Current => _current.Value?.Value;

    /// <summary>
    /// Pushes a new user context onto the stack.
    /// </summary>
    /// <param name="context">The user context to push.</param>
    /// <returns>A disposable object that will pop the context when disposed.</returns>
    public IDisposable Push(UserContext context)
    {
        var node = new Node
        {
            Value = context,
            Parent = _current.Value
        };
        _current.Value = node;
        return new Pop(this, node.Parent);
    }

    private sealed class Pop : IDisposable
    {
        private readonly UserContextAccessor _owner;
        private readonly Node? _restore;

        public Pop(UserContextAccessor owner, Node? restore)
        {
            _owner = owner;
            _restore = restore;
        }

        public void Dispose()
        {
            _owner._current.Value = _restore;
        }
    }

    /// <summary>
    /// Sets the current user context.
    /// </summary>
    /// <param name="context">The user context to set.</param>
    public void Set(UserContext context)
    {
        _current.Value = new Node
        {
            Value = context,
            Parent = null
        };
    }

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    public void Clear()
    {
        _current.Value = null;
    }
} 
