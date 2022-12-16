using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Components.Shared;
using Toolbelt.Blazor.HotKeys;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor.Server.UI.Services;
using BlazorState.Services;
using BlazorState;
using CleanArchitecture.Blazor.Application.Features.Identity.Profile;

namespace Blazor.Server.UI.Shared;

public partial class MainLayout: LayoutComponentBase,IDisposable, IBlazorStateComponent
{
    private bool _commandPaletteOpen;
    private HotKeysContext? _hotKeysContext;
    private bool _sideMenuDrawerOpen = true;
    private UserPreferences UserPreferences = new();
    public string Id => Guid.NewGuid().ToString();
    [Inject] 
    private LayoutService _layoutService { get; set; } = null!;
    [Inject] public IMediator Mediator { get; set; } = null!;
    [Inject] public IStore Store { get; set; } = null!;
    public void ReRender() => StateHasChanged();
    private MudThemeProvider? _mudThemeProvider { get; set; }=null!;
    private bool _themingDrawerOpen;
    [Inject] private IDialogService _dialogService { get; set; } = default!;
    [Inject] private HotKeys _hotKeys { get; set; } = default!;

    public void Dispose()
    {
       
        _layoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        _hotKeysContext?.Dispose();
        GC.SuppressFinalize(this);
    }
  
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ApplyUserPreferences();
            StateHasChanged();
        }
       
    }
    private async Task ApplyUserPreferences()
    {
            var defaultDarkMode = await _mudThemeProvider!.GetSystemPreference();
            UserPreferences = await _layoutService.ApplyUserPreferences(defaultDarkMode);
    }
    protected override async Task OnInitializedAsync()
    {
        _layoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
        _layoutService.SetBaseTheme(Theme.ApplicationTheme());
        _hotKeysContext = _hotKeys.CreateContext()
            .Add(ModKeys.Meta, Keys.K, OpenCommandPalette, "Open command palette.");
       var pf= Store.GetState<UserProfileState>();
       await base.OnInitializedAsync();

    }



    private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
   


    protected void SideMenuDrawerOpenChangedHandler(bool state)
    {
        _sideMenuDrawerOpen = state;
    }
    protected void ThemingDrawerOpenChangedHandler(bool state)
    {
        _themingDrawerOpen = state;
    }
    protected void ToggleSideMenuDrawer()
    {
        _sideMenuDrawerOpen = !_sideMenuDrawerOpen;
    }
    private async Task OpenCommandPalette()
    {
        if (!_commandPaletteOpen)
        {
            var options = new DialogOptions
            {
                NoHeader = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };

            var commandPalette = _dialogService.Show<CommandPalette>("", options);
            _commandPaletteOpen = true;

            await commandPalette.Result;
            _commandPaletteOpen = false;
        }
    }

    
}