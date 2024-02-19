using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using static CleanArchitecture.Blazor.Application.Features.Identity.DTOs.ApplicationUserDto;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class RegisterFormModelFluentValidator : AbstractValidator<RegisterFormModel>
{
    private readonly IIdentitySettings _identitySettings;
    private readonly IStringLocalizer<ApplicationUserDtoValidator> _localizer;

    public RegisterFormModelFluentValidator(
        IStringLocalizer<ApplicationUserDtoValidator> localizer,
        IIdentitySettings identitySettings)
    {
        _localizer = localizer;
        _identitySettings = identitySettings;

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_localizer["User name cannot be empty"])
            .Length(2, 100).WithMessage(_localizer["User name must be between 2 and 100 characters"]);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizer["E-mail cannot be empty"])
            .MaximumLength(100).WithMessage(_localizer["E-mail must be less than 100 characters"])
            .EmailAddress().WithMessage(_localizer["E-mail must be a valid email address"]);
        RuleFor(p => p.Password).NotEmpty().WithMessage(_localizer["Password cannot be empty"])
            .MinimumLength(_identitySettings.RequiredLength)
            .WithMessage(_localizer["Password must be at least {0} characters long", _identitySettings.RequiredLength])
            .MaximumLength(_identitySettings.MaxLength)
            .WithMessage(_localizer["Password cannot be longer than {0} characters", _identitySettings.MaxLength])
            .Matches(_identitySettings.RequireUpperCase ? @"[A-Z]+" : string.Empty)
            .WithMessage(_localizer["Password must contain at least one uppercase letter"])
            .Matches(_identitySettings.RequireLowerCase ? @"[a-z]+" : string.Empty)
            .WithMessage(_localizer["Password must contain at least one lowercase letter"])
            .Matches(_identitySettings.RequireDigit ? @"[0-9]+" : string.Empty)
            .WithMessage(_localizer["Password must contain at least one digit"])
            .Matches(_identitySettings.RequireNonAlphanumeric ? @"[\@\!\?\*\.]+" : string.Empty)
            .WithMessage(_localizer["Password must contain at least one non-alphanumeric character (e.g., @, !, ?, *, .)"]);
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage(_localizer["Confirm password must match the password"]);

        RuleFor(x => x.AgreeToTerms)
            .Equal(true).WithMessage(_localizer["You must agree to the terms and conditions"]);
    }
}