namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.UserActivation;

public record UserActivationNotification(string ActivationUrl, string Email, string UserId, string UserName)
    : INotification;

public class UserActivationNotificationHandler : INotificationHandler<UserActivationNotification>
{
    private readonly IStringLocalizer<UserActivationNotificationHandler> _localizer;
    private readonly ILogger<UserActivationNotificationHandler> _logger;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;

    public UserActivationNotificationHandler(
        ILogger<UserActivationNotificationHandler> logger,
        IStringLocalizer<UserActivationNotificationHandler> localizer,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _logger = logger;
        _localizer = localizer;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task Handle(UserActivationNotification notification, CancellationToken cancellationToken)
    {
        var sendMailResult = await _mailService.SendAsync(
            notification.Email,
            _localizer["Account Activation Required"],
            "_useractivation",
            new
            {
                notification.ActivationUrl,
                _settings.AppName,
                _settings.Company,
                notification.UserName,
                notification.Email
            });
        _logger.LogInformation("Activation email sent to {Email}, Activation Callback URL: {ActivationUrl}. sending result {Successful} {Message}, ",
            notification.Email, notification.ActivationUrl,sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
    }
}