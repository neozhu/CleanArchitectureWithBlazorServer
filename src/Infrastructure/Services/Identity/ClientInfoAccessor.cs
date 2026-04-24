namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IClientInfoAccessor using AsyncLocal for call chain isolation.
/// </summary>
public class ClientInfoAccessor : IClientInfoAccessor
{
    private sealed class Node
    {
        public ClientInfo? Value;
        public Node? Parent;
    }

    private readonly AsyncLocal<Node?> _current = new();

    /// <summary>
    /// Gets the current client information.
    /// </summary>
    public ClientInfo? Current => _current.Value?.Value;

    /// <summary>
    /// Pushes new client information onto the stack.
    /// </summary>
    /// <param name="clientInfo">The client information to push.</param>
    /// <returns>A disposable object that will pop the information when disposed.</returns>
    public IDisposable Push(ClientInfo? clientInfo)
    {
        var node = new Node
        {
            Value = clientInfo,
            Parent = _current.Value
        };

        _current.Value = node;

        return new Pop(this, node.Parent);
    }

    private sealed class Pop : IDisposable
    {
        private readonly ClientInfoAccessor _owner;
        private readonly Node? _restore;

        public Pop(ClientInfoAccessor owner, Node? restore)
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
    /// Sets the current client information.
    /// </summary>
    /// <param name="clientInfo">The client information to set.</param>
    public void Set(ClientInfo? clientInfo)
    {
        _current.Value = new Node
        {
            Value = clientInfo,
            Parent = null
        };
    }

    /// <summary>
    /// Clears the current client information.
    /// </summary>
    public void Clear()
    {
        _current.Value = null;
    }
}
