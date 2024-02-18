using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class ChangePasswordModel
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
{
    private readonly IIdentitySettings _identitySettings;
    private readonly IStringLocalizer<ChangePasswordModelValidator> _localizer;

    public ChangePasswordModelValidator(IIdentitySettings identitySettings,
         IStringLocalizer<ChangePasswordModelValidator> localizer)
    {
        _identitySettings = identitySettings;
        _localizer = localizer;
        RuleFor(p => p.NewPassword).NotEmpty().WithMessage(_localizer["CannotBeEmpty"])
           .MinimumLength(_identitySettings.RequiredLength)
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
            .Equal(x => x.NewPassword);
       
    }
}