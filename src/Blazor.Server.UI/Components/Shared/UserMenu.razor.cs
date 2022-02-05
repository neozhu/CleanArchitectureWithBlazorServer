using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudDemo.Server.Models;

namespace MudDemo.Server.Components.Shared;

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