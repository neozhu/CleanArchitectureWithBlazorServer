using Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;
using Microsoft.AspNetCore.Components.Web;


namespace Blazor.Server.UI.Components.Shared;

public partial class UserMenu:INotificationHandler<UpdateUserProfileCommand>
{

   
    private UserProfile? UserProfile { get; set; } = null!;
    [Parameter] public EventCallback<MouseEventArgs> OnSettingClick { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;
    private async Task OnLogout()
    {
        var parameters = new DialogParameters
            {
                { nameof(LogoutConfirmation.ContentText), $"{ConstantString.LOGOUTCONFIRMATION}"},
                { nameof(LogoutConfirmation.Color), Color.Error}
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = DialogService.Show<LogoutConfirmation>(ConstantString.LOGOUTCONFIRMATIONTITLE, parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await  TokenProvider.RemoveAuthDataFromStorage();
            NavigationManager.NavigateTo("/", true);
        }
    }
    protected override void OnInitialized()
    {
        UserProfileChanged += (s, e) =>
        {
            UserProfile = e.UserProfile;
            InvokeAsync(() => StateHasChanged());
        };
    }

    public static event EventHandler<UpdateUserProfileEventArgs> UserProfileChanged = null!;
    public Task Handle(UpdateUserProfileCommand notification, CancellationToken cancellationToken)
    {
        UserProfileChanged?.Invoke(this, new UpdateUserProfileEventArgs() { UserProfile = notification.UserProfile });
        return Task.CompletedTask;
    }
}