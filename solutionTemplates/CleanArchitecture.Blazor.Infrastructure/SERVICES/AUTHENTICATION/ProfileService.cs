using CleanArchitecture.Blazor.$safeprojectname$.Extensions;
namespace CleanArchitecture.Blazor.$safeprojectname$.Services.Authentication;

public class ProfileService 
{
    public event Action? OnChange;
    public UserProfile Profile { get; private set; } = new();
    public Task Set(ClaimsPrincipal principal)
    {
        Profile =  new UserProfile()
        {
            IsActive = principal.GetStatus(),
            TenantId = principal.GetTenantId(),
            TenantName = principal.GetTenantName(),
            Avatar = principal.GetProfilePictureDataUrl(),
            DisplayName = principal.GetDisplayName(),
            Email = principal.GetEmail(),
            PhoneNumber = principal.GetPhoneNumber(),
            Provider= principal.GetProvider(),
            Role = principal.GetRoles().FirstOrDefault(),
            UserId = principal.GetUserId(),
            UserName = principal.GetUserName(),
        };
        OnChange?.Invoke();
        return Task.CompletedTask;
    }
    public Task Update(UserProfile profile)
    {
        Profile = profile;
        OnChange?.Invoke();
        return Task.CompletedTask;
    }

   
}
