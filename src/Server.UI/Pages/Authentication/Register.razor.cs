using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Authentication;
public partial class Register
{
    public string Title = "Sign Up";
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private UserManager<ApplicationUser> UserManager { get; set; } = default!;

    private MudForm? _form;
    private bool _loading;
    private readonly RegisterFormModel _model = new();
    [Inject]
    private IMailService MailService { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        UserManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        return base.OnInitializedAsync();
    }
    private async Task Submit()
    {
        try
        {
            _loading = true;
            await _form!.Validate();
            if (_form!.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = _model.UserName,
                    Email = _model.Email
                };
                var result = await UserManager.CreateAsync(user, _model.Password!);
                if (result.Succeeded)
                {
                    var assignResult = await UserManager.AddToRoleAsync(user, RoleName.Basic);
                    if (assignResult.Succeeded && !string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.UserName))
                    {
                        var response = await SendWelcome(user.Email!, user.UserName!);
                        if (response.Successful == false)
                        {
                            Snackbar.Add(string.Format(L["{0}"], response.ErrorMessages.FirstOrDefault()), MudBlazor.Severity.Warning);
                        }
                        Snackbar.Add(L["Register successfully!"], MudBlazor.Severity.Info);
                        Navigation.NavigateTo("/");
                    }
                    else
                    {
                        Snackbar.Add($"{string.Join(",", result.Errors.Select(x => x.Description))}", MudBlazor.Severity.Error);
                    }
                }
                else
                {
                    Snackbar.Add($"{string.Join(",", result.Errors.Select(x => x.Description))}", MudBlazor.Severity.Error);
                }
            }

        }
        finally
        {
            _loading = false;
        }
    }

    private Task<SendResponse> SendWelcome(string toEmail, string userName)
    {

        var subject = string.Format(L["Welcome to {0}"], Settings.AppName);
        var template = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates", "_welcome.txt");
        return MailService.SendAsync(toEmail, subject, "_welcome", new { AppName = Settings.AppName, Email = toEmail, UserName = userName });
    }
}