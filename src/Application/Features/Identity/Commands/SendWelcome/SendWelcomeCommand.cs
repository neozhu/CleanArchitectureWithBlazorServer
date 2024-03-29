using System.Text;
using System.Web;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Identity;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Commands.SendWelcome;

public record SendWelcomeCommand(string Email) : IRequest<Result>;

public class SendWelcomeCommandHandler : IRequestHandler<SendWelcomeCommand, Result>
{
    private readonly IStringLocalizer<SendWelcomeCommandHandler> _localizer;
    private readonly IMailService _mailService;
    private readonly IApplicationSettings _settings;
    private readonly UserManager<ApplicationUser> _userManager;
    private string LoginUrl = "";
    public SendWelcomeCommandHandler(UserManager<ApplicationUser> userManager,
        IStringLocalizer<SendWelcomeCommandHandler> localizer,
        IMailService mailService,
        IApplicationSettings settings)
    {
        _userManager = userManager;
        _localizer = localizer;
        _mailService = mailService;
        _settings = settings;
    }


    public async Task<Result> Handle(SendWelcomeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) return Result.Failure(_localizer["No user found by email, please contact the administrator"]);

        var subject = string.Format(_localizer["Welcome to {0}"], _settings.AppName);
        LoginUrl = $"{_settings.ApplicationUrl}/pages/authentication/login";
        var sendMailResult = await _mailService.SendAsync(
            request.Email,
            subject,
            "_welcome",
            new { LoginUrl, _settings.AppName, user.Email, user.UserName, _settings.Company });

        return sendMailResult.Successful
            ? Result.Success()
            : Result.Failure(string.Format(_localizer["{0}, please contact the administrator"],
                sendMailResult.ErrorMessages.FirstOrDefault()));
    }
}