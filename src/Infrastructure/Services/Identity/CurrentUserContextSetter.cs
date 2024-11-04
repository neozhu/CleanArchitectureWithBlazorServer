namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Service for setting and clearing the current user context.
/// </summary>
public class CurrentUserContextSetter : ICurrentUserContextSetter
{
    private readonly ICurrentUserContext _currentUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserContextSetter"/> class.
    /// </summary>
    /// <param name="currentUserContext">The current user context.</param>
    public CurrentUserContextSetter(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// Sets the current user context with the provided session information.
    /// </summary>
    /// <param name="sessionInfo">The session information of the current user.</param>
    public void SetCurrentUser(SessionInfo sessionInfo)
    {
        _currentUserContext.SessionInfo = sessionInfo;
    }

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    public void Clear()
    {
        _currentUserContext.SessionInfo = null;
    }
}

