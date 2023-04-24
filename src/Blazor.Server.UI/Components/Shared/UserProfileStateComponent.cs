using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Server.UI.Components.Shared;

public class UserProfileStateComponent : ComponentBase, INotificationHandler<UpdateUserProfileCommand>
{
    private static event EventHandler<UpdateUserProfileEventArgs> UserProfileChanged = null!;

    public UserProfile? UserProfile { get; private set; } = null!;
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject]
    private IIdentityService IdentityService { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        UserProfileChanged += UserProfileChangedHandler;
        AuthenticationStateProvider.AuthenticationStateChanged += _authenticationStateProvider_AuthenticationStateChanged;
        var state = await AuthState;
        if (state?.User?.Identity?.IsAuthenticated ?? false)
        {
            var userDto = await IdentityService.GetApplicationUserDto(state.User.GetUserId());
            await SetProfile(userDto);
        }
        
    }
    private void _authenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> authenticationState)
    {
        InvokeAsync(async () =>
        {
            var state = await authenticationState;
            if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
            {
                var userDto = await IdentityService.GetApplicationUserDto(state.User.GetUserId());
                await SetProfile(userDto);
            }
        });
    }
    private Task SetProfile(ApplicationUserDto userDto)
    {
        UserProfile = userDto.ToUserProfile();
        return Task.CompletedTask;
    }
    public void Dispose()
    {
        UserProfileChanged -= UserProfileChangedHandler;
        AuthenticationStateProvider.AuthenticationStateChanged -= _authenticationStateProvider_AuthenticationStateChanged;
    }
    private void UserProfileChangedHandler(object? sender, UpdateUserProfileEventArgs e)
    {
        UserProfile = e.UserProfile;
        InvokeAsync(() => StateHasChanged());
    }
    public Task Handle(UpdateUserProfileCommand notification, CancellationToken cancellationToken)
    {
        UserProfileChanged?.Invoke(this, new UpdateUserProfileEventArgs() { UserProfile = notification.UserProfile });
        return Task.CompletedTask;
    }
}
