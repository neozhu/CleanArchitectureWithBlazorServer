using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

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
    private readonly IStringLocalizer<ApplicationUserDtoValidator> _localizer;

    public ChangePasswordModelValidator(IIdentitySettings identitySettings,
        IStringLocalizer<ApplicationUserDtoValidator> localizer)
    {
        _identitySettings = identitySettings;
        _localizer = localizer;
        RuleFor(p => p.NewPassword).NotEmpty().WithMessage(_localizer["New password cannot be empty"])
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
            .WithMessage(
                _localizer["Password must contain at least one non-alphanumeric character (e.g., @, !, ?, *, .)"]);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage(_localizer["Confirm password must match the new password"]);
    }
}