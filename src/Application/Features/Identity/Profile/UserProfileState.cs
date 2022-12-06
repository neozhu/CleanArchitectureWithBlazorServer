
using BlazorState;
namespace CleanArchitecture.Blazor.Application.Features.Identity.Profile;


public partial class UserProfileState : State<UserProfileState>
{
    private readonly ICurrentUserService _currentUserService;

    public UserProfileState(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
    public UserProfile UserProfile { get; private set; }
    public override void Initialize()
    {
        UserProfile = new UserProfile()
        {
            DisplayName= _currentUserService.DisplayName,
            UserName = _currentUserService.UserName,
            Email = _currentUserService.Email,
            TenantId = _currentUserService.TenantId,
            TenantName = _currentUserService.TenantName,
            UserId = _currentUserService.UserId,
            AssignRoles = _currentUserService.AssignRoles,
            ProfilePictureDataUrl = _currentUserService.ProfilePictureDataUrl,
            Role= _currentUserService.AssignRoles.FirstOrDefault(),
        };
    }
}
