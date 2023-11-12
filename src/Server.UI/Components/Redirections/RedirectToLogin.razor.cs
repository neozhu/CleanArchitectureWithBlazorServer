using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Components.Redirections;

public class RedirectToLogin : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        if (authState?.User?.Identity is null || !authState.User.Identity.IsAuthenticated)
        {
            var returnUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);

            if (string.IsNullOrWhiteSpace(returnUrl))
                NavigationManager.NavigateTo("/pages/authentication/login");
            else
                NavigationManager.NavigateTo($"/pages/authentication/login?returnUrl={returnUrl}");
        }
    }
}