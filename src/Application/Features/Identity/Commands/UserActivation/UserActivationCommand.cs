using System.Text;
using System.Web;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Commands.UserActivation;

public record UserActivationCommand(string Email) : IRequest<Result>;

public class UserActivationCommandHandler : IRequestHandler<UserActivationCommand, Result>
{
    private readonly IStringLocalizer<UserActivationCommandHandler> _localizer;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Logger<UserActivationCommandHandler> _logger;
    private string ActivationUrl = "";
    public UserActivationCommandHandler(UserManager<ApplicationUser> userManager,
        Logger<UserActivationCommandHandler> logger,
        IStringLocalizer<UserActivationCommandHandler> localizer,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _userManager = userManager;
        _logger = logger;
        _localizer = localizer;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task<Result> Handle(UserActivationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) return Result.Failure(_localizer["No user found by email, please contact the administrator"]);

        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmToken));
        //var template = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates" ,"_recoverypassword.txt");
        ActivationUrl = $"{_settings.ApplicationUrl}/pages/authentication/ConfirmEmail?code={code}&userid={user.Id}&returnUrl=/";
         var sendMailResult = await _mailService.SendAsync(
            request.Email,
            _localizer["Account Activation Required"],
            "_useractivation",
            new
            {
                ActivationUrl,
                _settings.AppName,
                _settings.Company,
                user.UserName,
                request.Email,
            });
        _logger.LogInformation("Activation email sent to {to}. sending result {Result} {Message}", request.Email, sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
        return sendMailResult.Successful
            ? Result.Success()
            : Result.Failure(string.Format(_localizer["{0}, please contact the administrator"],
                sendMailResult.ErrorMessages.FirstOrDefault()));
    }
}