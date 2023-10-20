using CleanArchitecture.Blazor.Server.UI.Services.Layout;
using Microsoft.AspNetCore.Components.Routing;

namespace CleanArchitecture.Blazor.Server.UI;
public partial class App
{

    [Inject]
    private LayoutService LayoutService { get; set; } = null!;

    private string _transitionClass = string.Empty;
    private bool _isLoaded = true;

    protected override async Task OnInitializedAsync()
    {
        _transitionClass = Settings.EnableLoadingScreen ? "loadScreen-initial-hidden" : string.Empty;
        LayoutService.SetBaseTheme(Theme.Theme.ApplicationTheme());
        await LayoutService.ApplyUserPreferences(true);

        if (Settings.EnableLoadingTransitionScreen)
        {
            NavManager.LocationChanged += HandleLocationChanged;
        }
    }
    public void Dispose()
    {
        if (Settings.EnableLoadingTransitionScreen)
        {
            NavManager.LocationChanged -= HandleLocationChanged;
        }
        GC.SuppressFinalize(this);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Settings.EnableLoadingScreen)
            {
                _transitionClass = "";
                _isLoaded = false;
                StateHasChanged();
                await Task.Delay(Settings.LoadingScreenDuration >= 0 ? Settings.LoadingScreenDuration : 2000);
                _isLoaded = true;
                StateHasChanged();
            }
        }
    }

    private async void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (Settings.EnableLoadingTransitionScreen)
        {
            _transitionClass = "";
            _isLoaded = false;
            StateHasChanged();
            await Task.Delay(Settings.LoadingTransitionScreenDuration >= 0 ? Settings.LoadingTransitionScreenDuration : 600);
            _isLoaded = true;
            StateHasChanged();
        }
    }

}