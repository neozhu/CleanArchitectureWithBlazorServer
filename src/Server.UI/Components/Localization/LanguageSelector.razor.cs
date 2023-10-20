using System.Globalization;
using CleanArchitecture.Blazor.Server.UI.Services.Layout;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Blazor.Server.UI.Components.Localization;
public partial class LanguageSelector
{
    public string? CurrentLanguage { get; set; } = "en-US";
    public List<CultureInfo>? SupportedLanguages { get; set; } = new();
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IOptions<RequestLocalizationOptions> LocalizationOptions { get; set; } = null!;
    [Inject] private LayoutService LayoutService { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        SupportedLanguages = LocalizationOptions.Value.SupportedCultures?.ToList();
        CurrentLanguage = CultureInfo.CurrentCulture.Name;
        return Task.CompletedTask;
    }


    private async Task ChangeLanguageAsync(string languageCode)
    {
        CurrentLanguage = languageCode;
        Navigation.NavigateTo(Navigation.BaseUri + "?culture=" + languageCode, forceLoad: true);

        if (new CultureInfo(languageCode).TextInfo.IsRightToLeft)
            await LayoutService.SetRightToLeft();
        else
            await LayoutService.SetLeftToRight();

        await Task.CompletedTask;
    }
}