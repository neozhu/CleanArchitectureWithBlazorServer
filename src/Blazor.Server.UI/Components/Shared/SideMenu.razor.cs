using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Models.SideMenu;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Application.Common.Models;
using Microsoft.AspNetCore.Components.Authorization;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using BlazorState;
using CleanArchitecture.Blazor.Application.Features.Identity.Profile;

namespace Blazor.Server.UI.Components.Shared;

public partial class SideMenu:BlazorStateComponent,IDisposable
{
    UserProfileState UserProfileState => GetState<UserProfileState>();
    private UserProfile UserProfile => UserProfileState.UserProfile;

    [EditorRequired] [Parameter] 
    public bool SideMenuDrawerOpen { get; set; } 
    
    [EditorRequired] [Parameter]
    public EventCallback<bool> SideMenuDrawerOpenChanged { get; set; }

    [Inject] 
    private IMenuService _menuService { get; set; } = default!;
    private IEnumerable<MenuSectionModel> _menuSections => _menuService.Features;
    
    [Inject] 
    private LayoutService LayoutService { get; set; } = default!;

    private string[] _roles => UserProfile.AssignRoles??new string[] { };






}