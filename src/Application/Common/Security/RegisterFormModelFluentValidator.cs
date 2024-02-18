using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class RegisterFormModelFluentValidator : AbstractValidator<RegisterFormModel>
{
    private readonly IIdentitySettings _identitySettings;
    private readonly IStringLocalizer<RegisterFormModelFluentValidator> _localizer;

    public RegisterFormModelFluentValidator(
        IStringLocalizer<RegisterFormModelFluentValidator> localizer,
        IIdentitySettings identitySettings)
    {
        _localizer = localizer;
        _identitySettings = identitySettings;

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(2, 100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(255)
            .EmailAddress();
        RuleFor(p => p.Password).NotEmpty().WithMessage(_localizer["CannotBeEmpty"])
            .MinimumLength(_identitySettings!.RequiredLength)
            .WithMessage(string.Format(_localizer["MinLength"], _identitySettings.RequiredLength))
            .MaximumLength(_identitySettings.MaxLength)
            .WithMessage(string.Format(_localizer["MaxLength"], _identitySettings.MaxLength))
            .Matches(_identitySettings.RequireUpperCase ? @"[A-Z]+" : string.Empty)
            .WithMessage(_localizer["MustContainUpperCase"])
            .Matches(_identitySettings.RequireLowerCase ? @"[a-z]+" : string.Empty)
            .WithMessage(_localizer["MustContainLowerCase"])
            .Matches(_identitySettings.RequireDigit ? @"[0-9]+" : string.Empty)
            .WithMessage(_localizer["MustContainDigit"])
            .Matches(_identitySettings.RequireNonAlphanumeric ? @"[\@\!\?\*\.]+" : string.Empty)
            .WithMessage(_localizer["MustContainNonAlphanumericCharacter"]);
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password);
        RuleFor(x => x.AgreeToTerms)
            .Equal(true);
    }
}