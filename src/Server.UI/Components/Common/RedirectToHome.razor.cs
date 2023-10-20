using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Components.Common;
public class RedirectToHome : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState> _authStateTask { get; set; } = null!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await _authStateTask;
        if (authState?.User?.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            //await Task.Delay(2000);
            NavigationManager.NavigateTo("/");
        }
    }
}
