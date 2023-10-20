using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using CleanArchitecture.Blazor.Server.UI.Pages.Identity.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Authentication;
public partial class Login
{
    private string _title = "Sign In";
    private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    private IAccessTokenProvider AccessTokenProvider { get; set; } = default!;

    [Inject]
    private ILogger<Login> Logger { get; set; } = default!;

    [Inject]
    private IDataProtectionProvider DataProtectionProvider { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject]
    protected IIdentityService _identityService { get; set; } = null!;

    private LoginFormModel _model = new()
    {
        UserName = "administrator",
        Password = "Password123!",
        RememberMe = true
    };
    private MudForm? _form;
    private bool _success;
    private bool _loading;

    [Inject]
    private LoginFormModelFluentValidator LoginValidator { get; set; } = default!;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _title = L["Sign In"];
        UserManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        AccessTokenProvider = ScopedServices.GetRequiredService<IAccessTokenProvider>();
    }

    private async Task OnSubmit()
    {
        try
        {
            _loading = true;
            await _form!.Validate();
            if (_form!.IsValid)
            {
                var user = await UserManager.FindByNameAsync(_model.UserName!);
                if (user is null)
                {
                    Logger.LogWarning("Someone tried to login to the user {@UserName:l}, however this account does not exist", _model.UserName);
                    Snackbar.Add(L["No user found, or no authorization, please contact the administrator."], Severity.Error);
                }
                else
                {
                    if (user.IsActive == false)
                    {
                        await OnResetPassword(user);
                    }
                    else
                    {
                        var result = await UserManager.CheckPasswordAsync(user, _model.Password!);
                        if (!result)
                        {
                            Logger.LogWarning("{@UserName:l} failed authentication", user.UserName);
                            Snackbar.Add(L["Please check your username and password. If you are still unable to log in, contact an administrator."], Severity.Error);
                        }
                        else
                        {

                            await TokenProvider.Login(user);
                            NavigationManager.NavigateTo(NavigationManager.Uri, true);
                            Logger.LogInformation("{@UserName:l} has successfully logged in", user.UserName);


                        }
                    }
                }
            }
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnResetPassword(ApplicationUser item)
    {
        var model = new ResetPasswordFormModel { Id = item.Id, DisplayName = item.DisplayName, UserName = item.UserName, ProfilePictureDataUrl = item.ProfilePictureDataUrl };
        var parameters = new DialogParameters<_ResetPasswordDialog> {
            { x=>x.Model, model}
        };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall };
        var dialog = DialogService.Show<_ResetPasswordDialog>(L["Set new password"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var token = await UserManager.GeneratePasswordResetTokenAsync(item);
            var state = await UserManager.ResetPasswordAsync(item, token, model.Password!);
            if (state.Succeeded)
            {
                item.IsActive = true;
                await UserManager.UpdateAsync(item);
                Snackbar.Add($"{L["Password update successfuly"]}", Severity.Info);
                Logger.LogInformation("{@UserName:l} has set a new password", item.UserName);
                _model.Password = "";
            }
            else
            {
                Snackbar.Add($"{string.Join(",", state.Errors.Select(x => x.Description).ToArray())}", Severity.Error);
            }
        }
    }

    private async Task SignInExternal(string provider)
    {
        await JS.InvokeVoidAsync("externalLogin", provider, DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public async Task ConfirmExternal(string provider, string userName, string name, string accessToken)
    {
        var user = await CreateUserWithExternalProvider(provider, userName, name, accessToken);
        await TokenProvider.Login(user);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
        Logger.LogInformation("{@UserName:l} has successfully logged in", userName);
    }

    private async Task<ApplicationUser> CreateUserWithExternalProvider(string provider, string userName, string name, string accessToken)
    {
        var user = await UserManager.FindByNameAsync(userName);
        if (user is null)
        {
            var admin = await UserManager.FindByNameAsync(UserName.Administrator) ?? throw new NotFoundException("Administrator's account Not Found.");
            user = new ApplicationUser
            {
                EmailConfirmed = true,
                IsActive = true,
                IsLive = true,
                UserName = userName,
                Email = userName.Any(x => x == '@') ? userName : $"{userName}@{provider}.com",
                Provider = provider,
                DisplayName = name,
                SuperiorId = admin.Id,
                TenantId = admin.TenantId,
                TenantName = admin.TenantName
            };
            var createResult = await UserManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                var assignResult = await UserManager.AddToRoleAsync(user, RoleName.Basic);
            }
            await UserManager.AddLoginAsync(user, new UserLoginInfo(provider, userName, accessToken));
        }
        return user;
    }

}