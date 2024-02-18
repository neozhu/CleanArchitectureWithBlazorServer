using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class ResetPasswordFormModel
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class ResetPasswordFormModelValidator : AbstractValidator<ResetPasswordFormModel>
{
    private readonly IStringLocalizer<ResetPasswordFormModelValidator> _localizer;
    private readonly IIdentitySettings _identitySettings;

    public ResetPasswordFormModelValidator(IStringLocalizer<ResetPasswordFormModelValidator> localizer,
        IIdentitySettings identitySettings)
    {
        _localizer = localizer;
        _identitySettings = identitySettings;
        RuleFor(p => p.Password).NotEmpty().WithMessage(_localizer["CannotBeEmpty"])
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
            .Equal(x => x.Password);
       
    }

}