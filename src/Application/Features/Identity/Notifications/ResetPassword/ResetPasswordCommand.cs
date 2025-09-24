namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.ResetPassword;

public record ResetPasswordNotification(string RequestUrl, string Email, string UserName) : INotification;

public class ResetPasswordNotificationHandler : INotificationHandler<ResetPasswordNotification>
{
    private readonly IStringLocalizer<ResetPasswordNotificationHandler> _localizer;
    private readonly ILogger<ResetPasswordNotificationHandler> _logger;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;

    public ResetPasswordNotificationHandler(
        IStringLocalizer<ResetPasswordNotificationHandler> localizer,
        ILogger<ResetPasswordNotificationHandler> logger,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _localizer = localizer;
        _logger = logger;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task Handle(ResetPasswordNotification notification, CancellationToken cancellationToken)
    {
        await _mailService.SendAsync(
            notification.Email,
            _localizer["Verify your recovery email"],
            "_recoverypassword",
            new
            {
                RequestUrl = notification.RequestUrl,
                AppName = _settings.AppName,
                Company = _settings.Company,
                UserName = notification.UserName,
                Email = notification.Email
            });
        _logger.LogInformation("Password reset email sent to {Email}. Reset Password Callback URL: {RequestUrl}", 
            notification.Email, notification.RequestUrl);
    }
}
