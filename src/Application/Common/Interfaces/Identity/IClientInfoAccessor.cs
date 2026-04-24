namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

public interface IClientInfoAccessor
{
    /// <summary>
    /// Gets the current client information (IP address and browser details) for the current async call chain.
    /// </summary>
    ClientInfo? Current { get; }

    /// <summary>
    /// Pushes new client information onto the stack.
    /// </summary>
    IDisposable Push(ClientInfo? clientInfo);

    /// <summary>
    /// Sets the current client information, replacing any existing stack.
    /// </summary>
    void Set(ClientInfo? clientInfo);

    /// <summary>
    /// Clears the current client information.
    /// </summary>
    void Clear();
}
