using System.Net;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Components.Common;
public partial class CustomError
{
    private string? Message { get; set; }
    private bool ShowStackTrace { get; set; }
    private string? StackTrace { get; set; }
    private string? StatusCode { get; set; } = HttpStatusCode.InternalServerError.ToString();
    [EditorRequired][Parameter] public Exception Exception { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ILogger<CustomError> Logger { get; set; } = default!;
    [CascadingParameter]
    private Task<AuthenticationState> _authState { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        var state = await _authState;
        var userName = state.User.GetUserName();
        switch (Exception)
        {
            case ServerException e:
                StatusCode = e.StatusCode.ToString();
                if (e.ErrorMessages is not null)
                {
                    Message = string.Join(", ", e.ErrorMessages.ToArray());
                }
                break;
            default:
                if (Exception.InnerException != null)
                {
                    while (Exception.InnerException != null)
                    {
                        Exception = Exception.InnerException;
                    }
                }
                Message = Exception.Message;
                break;
        }
        StackTrace = Exception.StackTrace;
        Logger.LogError(Exception, "{Message}. request url: {@url} {@UserName}", Message, Navigation.Uri, userName);
    }
    private void OnRefresh()
    {
        Navigation.NavigateTo(Navigation.Uri, true);
    }
}