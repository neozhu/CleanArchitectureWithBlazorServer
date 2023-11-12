using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Components.Redirections;

public class RedirectToHome : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState> _authStateTask { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await _authStateTask;

        if (authState?.User?.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
