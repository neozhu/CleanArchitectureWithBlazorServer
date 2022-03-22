using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Models;
using Blazor.Server.UI.Services;

namespace Blazor.Server.UI.Components.Shared.Themes;

public partial class ThemesMenu
{
    // TODO: The razor file could probably be cleaner..
    // TODO: Find why MudOverlay does not work..
    // TODO: Better CSS..
    
    private readonly List<string> _primaryColors = new()
    {
        "#2d4275",
        Colors.Green.Default,
        Colors.Blue.Default,
        Colors.BlueGrey.Default,
        Colors.Purple.Default,
        Colors.Orange.Default,
        Colors.Red.Default
    };

    [EditorRequired] [Parameter] public bool ThemingDrawerOpen { get; set; }
    [EditorRequired] [Parameter] public EventCallback<bool> ThemingDrawerOpenChanged { get; set; }
    [EditorRequired] [Parameter] public UserPreferences UserPreferences { get; set; }
    [EditorRequired] [Parameter] public EventCallback<UserPreferences> UserPreferencesChanged { get; set; }

    [Parameter]
    public double Radius { get; set; }

    [Parameter]
    public double MaxValue { get; set; } = 32;

    private async Task UpdateThemePrimaryColor(string color)
    {
        UserPreferences.PrimaryColor = color;
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    private async Task ChangedSelection(ChangeEventArgs args)
    {
        UserPreferences.BorderRadius = int.Parse(args?.Value?.ToString() ?? "0");
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
    private async Task ToggleDarkLightMode(bool isDarkMode)
    {
        UserPreferences.IsDarkMode = isDarkMode;
        await UserPreferencesChanged.InvokeAsync(UserPreferences);
    }
}