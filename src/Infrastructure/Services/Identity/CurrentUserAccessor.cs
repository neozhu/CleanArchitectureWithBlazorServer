namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Provides access to the current user's session information.
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly ICurrentUserContext _currentUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserAccessor"/> class.
    /// </summary>
    /// <param name="currentUserContext">The current user context.</param>
    public CurrentUserAccessor(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// Gets the session information of the current user.
    /// </summary>
    public SessionInfo? SessionInfo => _currentUserContext.SessionInfo;
}

