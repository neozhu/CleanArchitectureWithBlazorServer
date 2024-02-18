using System.Web;
using CleanArchitecture.Blazor.Domain.Identity;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Commands.ResetPassword;

public record ResetPasswordCommand(string Email) : IRequest<Result>;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IStringLocalizer<ResetPasswordCommandHandler> _localizer;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;
    private string RequestUrl = "";
    public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager,
        IStringLocalizer<ResetPasswordCommandHandler> localizer,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _userManager = userManager;
        _localizer = localizer;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) return Result.Failure(_localizer["No user found by email, please contact the administrator"]);

        var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        //var template = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates" ,"_recoverypassword.txt");
        RequestUrl = $"{_settings.ApplicationUrl}/pages/authentication/reset-password/{user.Id}?token={HttpUtility.UrlEncode(resetPasswordToken)}";
         var sendMailResult = await _mailService.SendAsync(
            request.Email,
            _localizer["Verify your recovery email"],
            "_recoverypassword",
            new
            {
                RequestUrl,
                _settings.AppName,
                _settings.Company,
                user.UserName,
                request.Email,
            });

        return sendMailResult.Successful
            ? Result.Success()
            : Result.Failure(string.Format(_localizer["{0}, please contact the administrator"],
                sendMailResult.ErrorMessages.FirstOrDefault()));
    }
}