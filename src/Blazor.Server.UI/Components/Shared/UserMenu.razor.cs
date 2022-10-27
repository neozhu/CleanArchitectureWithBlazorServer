using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using CleanArchitecture.Blazor.Application.Constants;

namespace Blazor.Server.UI.Components.Shared;

public partial class UserMenu
{

    [EditorRequired] [Parameter] public UserProfile UserProfile { get; set; } = default!;
    [Parameter] public EventCallback<MouseEventArgs> OnSettingClick { get; set; }
    [Inject] private IdentityAuthenticationService _authenticationService { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
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
        if (!result.Cancelled)
        {
            await _authenticationService.Logout();
            await JS.InvokeVoidAsync("externalLogout");
        }
    } 
}