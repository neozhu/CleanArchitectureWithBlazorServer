namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.SendFactorCode;

public record SendFactorCodeNotification(string Email, string UserName, string AuthenticatorCode) : INotification;

public class SendFactorCodeNotificationHandler : INotificationHandler<SendFactorCodeNotification>
{
    private readonly IStringLocalizer<SendFactorCodeNotificationHandler> _localizer;
    private readonly ILogger<SendFactorCodeNotificationHandler> _logger;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;


    public SendFactorCodeNotificationHandler(
        IStringLocalizer<SendFactorCodeNotificationHandler> localizer,
        ILogger<SendFactorCodeNotificationHandler> logger,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _localizer = localizer;
        _logger = logger;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task Handle(SendFactorCodeNotification notification, CancellationToken cancellationToken)
    {
        var subject = _localizer["Your Verification Code"];
        await _mailService.SendAsync(
            notification.Email,
            subject,
            "_authenticatorcode",
            new
            {
                AuthenticatorCode = notification.AuthenticatorCode, 
                AppName = _settings.AppName, 
                Email = notification.Email, 
                UserName = notification.UserName,
                Company = _settings.Company
            });
        _logger.LogInformation("Verification Code email sent to {Email}. Authenticator Code:{AuthenticatorCode}", 
            notification.Email, notification.AuthenticatorCode);
    }
}
