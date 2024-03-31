using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.SendWelcome;

public record SendWelcomeNotification(string Email) : INotification;

public class SendWelcomeNotificationHandler : INotificationHandler<SendWelcomeNotification>
{
    private readonly IStringLocalizer<SendWelcomeNotificationHandler> _localizer;
    private readonly ILogger<SendWelcomeNotificationHandler> _logger;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;
    private string LoginUrl = "";
    public SendWelcomeNotificationHandler(UserManager<ApplicationUser> userManager,
        IStringLocalizer<SendWelcomeNotificationHandler> localizer,
        ILogger<SendWelcomeNotificationHandler> logger,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _userManager = userManager;
        _localizer = localizer;
        _logger = logger;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task Handle(SendWelcomeNotification notification, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(notification.Email);

        if (user == null)
        {
            _logger.LogError(string.Format(_localizer["Welcome notification sending failed. No user associated with email {Email}, Verify the email address or contact the administrator"]), notification.Email);
            return;
        }

        var subject = string.Format(_localizer["Welcome to {0}"], _settings.AppName);
        LoginUrl = $"{_settings.ApplicationUrl}/pages/authentication/login";
        var sendMailResult = await _mailService.SendAsync(
            notification.Email,
            subject,
            "_welcome",
            new { LoginUrl, _settings.AppName, user.Email, user.UserName, _settings.Company });
        _logger.LogInformation("Welcome email sent to {Email}. sending result {Successful} {ErrorMessages}", notification.Email, sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
    }
}