using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Server.UI.Components.Shared;

public class UserProfileStateComponent : ComponentBase, INotificationHandler<UpdateUserProfileCommand>
{
    private static event EventHandler<UpdateUserProfileEventArgs> _userProfileChanged = null!;

    public UserProfile? UserProfile { get; private set; } = null!;
    [CascadingParameter]
    protected Task<AuthenticationState> _authState { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
    [Inject]
    private IIdentityService _identityService { get; set; } = default!;
    [Inject]
    private IMediator _mediator { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        _userProfileChanged += userProfileChangedHandler;
        _authenticationStateProvider.AuthenticationStateChanged += _authenticationStateProvider_AuthenticationStateChanged;
        var state = await _authState;
        if (state?.User?.Identity?.IsAuthenticated ?? false)
        {
            var userDto = await _identityService.GetApplicationUserDto(state.User.GetUserId());
            await setProfile(userDto);
        }
        
    }
    private void _authenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> authenticationState)
    {
        InvokeAsync(async () =>
        {
            var state = await authenticationState;
            if (state.User.Identity != null && state.User.Identity.IsAuthenticated)
            {
                var userDto = await _identityService.GetApplicationUserDto(state.User.GetUserId());
                await setProfile(userDto);
            }
        });
    }
    private async Task setProfile(ApplicationUserDto userDto)
    {
        await _mediator.Publish(new UpdateUserProfileCommand() { UserProfile = userDto.ToUserProfile() });
    }
    public void Dispose()
    {
        _userProfileChanged -= userProfileChangedHandler;
        _authenticationStateProvider.AuthenticationStateChanged -= _authenticationStateProvider_AuthenticationStateChanged;
    }
    private void userProfileChangedHandler(object? sender, UpdateUserProfileEventArgs e)
    {
        UserProfile = e.UserProfile;
        InvokeAsync(() => StateHasChanged());
    }
    public Task Handle(UpdateUserProfileCommand notification, CancellationToken cancellationToken)
    {
        _userProfileChanged?.Invoke(this, new UpdateUserProfileEventArgs() { UserProfile = notification.UserProfile });
        return Task.CompletedTask;
    }
}
