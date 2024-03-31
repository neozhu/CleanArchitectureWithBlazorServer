using System.Text;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.ResetPassword;

public record ResetPasswordNotification(string Email) : INotification;

public class ResetPasswordNotificationHandler : INotificationHandler<ResetPasswordNotification>
{
    private readonly IStringLocalizer<ResetPasswordNotificationHandler> _localizer;
    private readonly ILogger<ResetPasswordNotificationHandler> _logger;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;
    private string RequestUrl = "";
    public ResetPasswordNotificationHandler(IServiceScopeFactory scopeFactory,
        IStringLocalizer<ResetPasswordNotificationHandler> localizer,
        ILogger<ResetPasswordNotificationHandler> logger,
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


    public async Task Handle(ResetPasswordNotification notification, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(notification.Email);

        if (user == null)
        {
            _logger.LogError("Password recovery notification sending failed. No user associated with email {Email}, Verify the email address or contact the administrator", notification.Email);
            return;
        }

        var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        RequestUrl = $"{_settings.ApplicationUrl}/pages/authentication/reset-password?userid={user.Id}&token={WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken))}";
        var sendMailResult = await _mailService.SendAsync(
           notification.Email,
           _localizer["Verify your recovery email"],
           "_recoverypassword",
           new
           {
               RequestUrl,
               _settings.AppName,
               _settings.Company,
               user.UserName,
               notification.Email,
           });
        _logger.LogInformation("Password rest email sent to {Email}. sending result {Successful} {ErrorMessages}", notification.Email, sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));

    }
}