namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface to access the current user's session information.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Gets the current session information of the user.
    /// </summary>
    SessionInfo? SessionInfo { get; }
}
