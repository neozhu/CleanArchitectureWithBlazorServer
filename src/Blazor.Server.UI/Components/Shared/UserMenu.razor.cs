using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Blazor.Server.UI.Models;

namespace Blazor.Server.UI.Components.Shared;

public partial class UserMenu
{
    [Parameter] public string Class { get; set; }
    [EditorRequired] [Parameter] public UserModel User { get; set; }
    [Inject] private NavigationManager _navigation { get; set; }
    private async Task Logout()
    {
       
        //_navigation.NavigateTo("/pages/authentication/login");
    } 
}