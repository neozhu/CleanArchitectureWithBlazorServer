using CleanArchitecture.Blazor.Infrastructure.Extensions;
namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public class ProfileService 
{
    public  Func<UserModel,Task>? OnChange;
    public UserModel Profile { get; private set; } = default!;
    public async Task<UserModel> Get(ClaimsPrincipal principal)
    {
        var user = principal;
        Profile =  new UserModel()
        {
            Avatar = user.GetProfilePictureDataUrl(),
            DisplayName = user.GetDisplayName(),
            Email = user.GetEmail(),
            PhoneNumber = user.GetPhoneNumber(),
            Site=user.GetSite(),
            Role = user.GetRoles().FirstOrDefault(),
            UserId = user.GetUserId(),
            UserName = user.GetUserName(),
        };
        return await Task.FromResult(Profile);
    }
    public Task Update(UserModel profile)
    {
        Profile = profile;
        OnChange?.Invoke(Profile);
        return Task.CompletedTask;
    }

   
}
