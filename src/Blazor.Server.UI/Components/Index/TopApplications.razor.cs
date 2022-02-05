using MudBlazor;
using MudBlazor.Utilities;
using Blazor.Server.UI.Models.Application;

namespace Blazor.Server.UI.Components.Index;

public partial class TopApplications : MudComponentBase
{
    private List<ApplicationModel> _applications = new()
    {
        new ApplicationModel
        {
            Title = "MudBlazor",
            Icon = Icons.Custom.Brands.MudBlazor,
            NumberOfReview = 4599,
            ReviewScore = 5
        },
        new ApplicationModel
        {
            Title = "Github",
            Icon = Icons.Custom.Brands.GitHub,
            NumberOfReview = 3548,
            ReviewScore = 5
        },
        new ApplicationModel
        {
            Title = "Azure",
            Icon = Icons.Custom.Brands.MicrosoftAzureDevOps,
            NumberOfReview = 844,
            ReviewScore = 4,
            Price = 9.99
        }
    };

    private string Classname =>
        new CssBuilder()
            .AddClass(Class)
            .Build();
}