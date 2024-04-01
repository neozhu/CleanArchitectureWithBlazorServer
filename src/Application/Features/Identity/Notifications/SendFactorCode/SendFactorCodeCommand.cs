using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.SendFactorCode;

public record SendFactorCodeNotification(string Email,string AuthenticatorCode) : INotification;

public class SendFactorCodeNotificationHandler : INotificationHandler<SendFactorCodeNotification>
{
    private readonly IStringLocalizer<SendFactorCodeNotificationHandler> _localizer;
    private readonly ILogger<SendFactorCodeNotificationHandler> _logger;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;

    public SendFactorCodeNotificationHandler(IServiceScopeFactory scopeFactory,
        IStringLocalizer<SendFactorCodeNotificationHandler> localizer,
        ILogger<SendFactorCodeNotificationHandler> logger,
        IMailService mailService,
        IApplicationSettings settings)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _localizer = localizer;
        _logger = logger;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task Handle(SendFactorCodeNotification notification, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(notification.Email);
        if (user == null)
        {
            _logger.LogError("Verification code notification sending failed. No user associated with email {Email}, Verify the email address or contact the administrator", notification.Email);
            return;
        }
        var subject = _localizer["Your Verification Code"];
        var sendMailResult = await _mailService.SendAsync(
            notification.Email,
            subject,
            "_authenticatorcode",
            new { notification.AuthenticatorCode, _settings.AppName, user.Email, user.UserName, _settings.Company });
        _logger.LogInformation("Verification Code email sent to {Email}. sending result {Successful} {ErrorMessages}", notification.Email, sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
     }
}