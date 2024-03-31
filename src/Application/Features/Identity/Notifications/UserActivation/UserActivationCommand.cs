using System.Text;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.UserActivation;

public record UserActivationNotification(string Email) : INotification;

public class UserActivationNotificationHandler : INotificationHandler<UserActivationNotification>
{
    private readonly IStringLocalizer<UserActivationNotificationHandler> _localizer;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserActivationNotificationHandler> _logger;
    private string ActivationUrl = "";
    public UserActivationNotificationHandler(IServiceScopeFactory scopeFactory,
        ILogger<UserActivationNotificationHandler> logger,
        IStringLocalizer<UserActivationNotificationHandler> localizer,
        IMailService mailService,
        IApplicationSettings settings)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _logger = logger;
        _localizer = localizer;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task Handle(UserActivationNotification notification, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(notification.Email);

        if (user == null)
        {
            _logger.LogError(string.Format(_localizer["Account activation notification sending failed. No user associated with email {Email}, Verify the email address or contact the administrator"]), notification.Email);
            return;
        }

        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmToken));
        ActivationUrl = $"{_settings.ApplicationUrl}/pages/authentication/ConfirmEmail?code={code}&userid={user.Id}&returnUrl=/";
         var sendMailResult = await _mailService.SendAsync(
            notification.Email,
            _localizer["Account Activation Required"],
            "_useractivation",
            new
            {
                ActivationUrl,
                _settings.AppName,
                _settings.Company,
                user.UserName,
                notification.Email,
            });
        _logger.LogInformation("Activation email sent to {Email}. sending result {Successful} {Message}", notification.Email, sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
 
    }
}