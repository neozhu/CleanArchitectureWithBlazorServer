using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Server.UI.Pages.Identity.Users;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Authentication;
public partial class Forgot
{
    public string Title = "Forgot Password";
    private int _step = 1;
    private string _emailAddress = string.Empty;
    private bool _sending = false;
    private string _resetPasswordToken = string.Empty;
    private string _inputToken = string.Empty;

    private ApplicationUser? _user = null;
    private UserManager<ApplicationUser> UserManager { get; set; } = null!;
    [Inject]
    private IMailService MailService { get; set; } = null!;
    [Inject] private ILogger<Forgot> Logger { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    protected override Task OnInitializedAsync()
    {
        Title = L["Forgot Password"];
        UserManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        return base.OnInitializedAsync();
    }
    private async Task ResetPassword()
    {
        try
        {
            _sending = true;
            _user = await UserManager.FindByEmailAsync(_emailAddress) ?? throw new NotFoundException($"Application user {_emailAddress} Not Found."); ;
            if (_user is null)
            {
                Snackbar.Add(L["No user found by email, please contact the administrator"], MudBlazor.Severity.Error);
                return;
            }
            _resetPasswordToken = await UserManager.GeneratePasswordResetTokenAsync(_user);
            var response = await SendResetPasswordToken(_emailAddress, _user.UserName!, _resetPasswordToken);
            if (response.Successful)
            {
                _step = 2;
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add(string.Format(L["The email has been sent, please check the email:{0} "], _emailAddress), MudBlazor.Severity.Success, config => { config.ShowCloseIcon = false; config.HideTransitionDuration = 5000; });
                Logger.LogInformation("{@UserName:l}'s password reset token: {@Token} has been sent", _user.UserName, "******");
            }
            else
            {
                Snackbar.Add(string.Format(L["{0}, please contact the administrator"], response.ErrorMessages.FirstOrDefault()), MudBlazor.Severity.Error);
            }

            // The following code is just for testing 
            // prompt token string
            //Snackbar.Add(_resetpasswordToken, MudBlazor.Severity.Warning);
            //_step = 2;

        }
        catch (Exception e)
        {
            Snackbar.Add(string.Format(L["{0}, please contact the administrator"], e.Message), MudBlazor.Severity.Error);
        }
        finally
        {
            _sending = false;
        }
    }

    private Task<SendResponse> SendResetPasswordToken(string toEmail, string userName, string token)
    {

        var subject = L["Verify your recovery email"];
        //var template = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates" ,"_recoverypassword.txt");
        return MailService.SendAsync(toEmail, subject, "_recoverypassword", new { AppName = Settings.AppName, Email = toEmail, Token = token });
    }


    private async Task SetNewPassword()
    {
        try
        {
            _sending = true;
            if (_user is null || _inputToken == string.Empty)
            {
                return;
            }
            var model = new ResetPasswordFormModel()
            { Id = _user.Id, DisplayName = _user.DisplayName, UserName = _user.UserName, ProfilePictureDataUrl = _user.ProfilePictureDataUrl };
            var parameters = new DialogParameters<_ResetPasswordDialog> {
            { x=>x.Model, model}
        };
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = DialogService.Show<_ResetPasswordDialog>(L["Set a new password"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var state = await UserManager.ResetPasswordAsync(_user, _inputToken, model.Password!);
                if (state.Succeeded)
                {
                    _user.IsActive = true;
                    await UserManager.UpdateAsync(_user);
                    Snackbar.Add($"{L["The new password has been set successfully, please login again"]}", MudBlazor.Severity.Info);
                    Logger.LogInformation("{@UserName:l} has set a new password", model.UserName);
                    Navigation.NavigateTo("/");
                }
                else
                {
                    Snackbar.Add($"{string.Join(",", state.Errors.Select(x => x.Description).ToArray())}", MudBlazor.Severity.Error);
                }
            }
        }
        catch (Exception e)
        {
            Snackbar.Add(string.Format(L["{0}, please contact the administrator"], e.Message), MudBlazor.Severity.Error);
        }
        finally
        {
            _sending = false;
        }
    }
}