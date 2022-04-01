using CleanArchitecture.Blazor.Infrastructure.Extensions;
namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public class ProfileService 
{
    public event Action? OnChange;
    public UserModel Profile { get; private set; } = new();
    public Task Set(ClaimsPrincipal principal)
    {
        Profile =  new UserModel()
        {
            Avatar = principal.GetProfilePictureDataUrl(),
            DisplayName = principal.GetDisplayName(),
            Email = principal.GetEmail(),
            PhoneNumber = principal.GetPhoneNumber(),
            Site= principal.GetSite(),
            Role = principal.GetRoles().FirstOrDefault(),
            UserId = principal.GetUserId(),
            UserName = principal.GetUserName(),
        };
        OnChange?.Invoke();
        return Task.CompletedTask;
    }
    public Task Update(UserModel profile)
    {
        Profile = profile;
        OnChange?.Invoke();
        return Task.CompletedTask;
    }

   
}
