using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Models.SideMenu;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Application.Common.Models;
using Microsoft.AspNetCore.Components.Authorization;
using CleanArchitecture.Blazor.Infrastructure.Extensions;

namespace Blazor.Server.UI.Components.Shared;

public partial class SideMenu:IDisposable
{
    private IEnumerable<MenuSectionModel> _menuSections = new List<MenuSectionModel>();

    [EditorRequired] [Parameter] 
    public bool SideMenuDrawerOpen { get; set; } 
    
    [EditorRequired] [Parameter]
    public EventCallback<bool> SideMenuDrawerOpenChanged { get; set; }
   
    [EditorRequired] [Parameter] 
    public UserProfile UserProfile { get; set; } = default!;

    [Inject] 
    private IMenuService _menuService { get; set; } = default!;
    
    [CascadingParameter]
    protected Task<AuthenticationState> _authState { get; set; } = default!;
    
    [Inject]
    private AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
    
    [Inject] 
    private LayoutService LayoutService { get; set; } = default!;
    
    private string[] _roles = new string[] { };
    
    protected override async Task OnInitializedAsync()
    {
        var authstate = await _authState;
        _roles = authstate.User.GetRoles();
        _menuSections = _menuService.Features;
        _authenticationStateProvider.AuthenticationStateChanged += _authenticationStateProvider_AuthenticationStateChanged;

    }
    private async void _authenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var authstate = await task;
        _roles = authstate.User.GetRoles();
    }
    public void Dispose()
    {
        _authenticationStateProvider.AuthenticationStateChanged -= _authenticationStateProvider_AuthenticationStateChanged;
    }
}