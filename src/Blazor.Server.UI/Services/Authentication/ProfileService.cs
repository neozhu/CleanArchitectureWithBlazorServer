using Blazor.Server.UI.Models;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Server.UI.Services.Authentication;

public class ProfileService 
{
    public  Func<UserModel,Task>? OnChange;
    public UserModel Profile { get; private set; } = default!;



    public async Task Set(Task<AuthenticationState> state)
    {
        var user = (await state).User;
        Profile =  new UserModel()
        {
            Avatar = user.GetProfilePictureDataUrl(),
            DisplayName = user.GetDisplayName(),
            Email = user.GetEmail(),
            PhoneNumber = user.GetPhoneNumber(),
            Site=user.GetSite(),
            Role = user.GetRoles().FirstOrDefault()
        };
        OnChange?.Invoke(Profile);
    }

    public Task Update(UserModel profile)
    {
        Profile = profile;
        OnChange?.Invoke(Profile);
        return Task.CompletedTask;
    }
}
