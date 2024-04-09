using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Server.UI.Fluxor;

public class UserProfileUpdateAction
{
    public required UserProfile UserProfile { get; set; }
}