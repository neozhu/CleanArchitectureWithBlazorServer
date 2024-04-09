namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.SendWelcome;

public record SendWelcomeNotification(string LoginUrl, string Email, string UserName) : INotification;

public class SendWelcomeNotificationHandler : INotificationHandler<SendWelcomeNotification>
{
    private readonly IStringLocalizer<SendWelcomeNotificationHandler> _localizer;
    private readonly ILogger<SendWelcomeNotificationHandler> _logger;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;


    public SendWelcomeNotificationHandler(
        IStringLocalizer<SendWelcomeNotificationHandler> localizer,
        ILogger<SendWelcomeNotificationHandler> logger,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _localizer = localizer;
        _logger = logger;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task Handle(SendWelcomeNotification notification, CancellationToken cancellationToken)
    {
        var subject = string.Format(_localizer["Welcome to {0}"], _settings.AppName);
        var sendMailResult = await _mailService.SendAsync(
            notification.Email,
            subject,
            "_welcome",
            new
            {
                notification.LoginUrl, _settings.AppName, notification.Email, notification.UserName, _settings.Company
            });
        _logger.LogInformation("Welcome email sent to {Email}. sending result {Successful} {ErrorMessages}",
            notification.Email, sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
    }
}