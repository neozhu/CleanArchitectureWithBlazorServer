using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.Server.UI.Components.Shared;

public partial class UserMenu
{
    [Parameter] public string Class { get; set; }
    [EditorRequired] [Parameter] public UserModel User { get; set; } = default!;
    [Parameter] public EventCallback<MouseEventArgs> OnSettingClick { get; set; }
    [Inject] private IdentityAuthenticationService _authenticationService { get; set; } = default!;
    private async Task OnLogout()
    {
        var parameters = new DialogParameters
            {
                { nameof(LogoutConfirmation.ContentText), $"{L["You are attempting to log out of application. Do you really want to log out?"]}"},
                { nameof(LogoutConfirmation.ButtonText), $"{L["Logout"]}"},
                { nameof(LogoutConfirmation.Color), Color.Error}
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = DialogService.Show<LogoutConfirmation>(L["Logout"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await _authenticationService.Logout();
        }
    } 
}