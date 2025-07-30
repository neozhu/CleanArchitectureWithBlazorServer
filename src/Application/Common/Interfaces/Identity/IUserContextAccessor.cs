namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface for accessing and managing user context with support for nested contexts.
/// </summary>
public interface IUserContextAccessor
{
    /// <summary>
    /// Gets the current user context.
    /// </summary>
    UserContext? Current { get; }

    /// <summary>
    /// Pushes a new user context onto the stack.
    /// </summary>
    /// <param name="context">The user context to push.</param>
    /// <returns>A disposable object that will pop the context when disposed.</returns>
    IDisposable Push(UserContext context);

    /// <summary>
    /// Pops the current user context from the stack.
    /// </summary>
    void Pop();

    /// <summary>
    /// Sets the current user context.
    /// </summary>
    /// <param name="context">The user context to set.</param>
    void Set(UserContext context);

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    void Clear();
} 