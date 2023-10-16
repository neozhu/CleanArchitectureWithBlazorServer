using FluentValidation.Results;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class LoginFormModel
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool RememberMe { get; set; } = false;
}

public class LoginFormModelFluentValidator : AbstractValidator<LoginFormModel>
{
    private readonly IStringLocalizer<LoginFormModelFluentValidator> _localizer;

    public LoginFormModelFluentValidator(IStringLocalizer<LoginFormModelFluentValidator> localizer)
    {
        _localizer = localizer;
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_localizer["Your username cannot be empty."])
            .Length(2, 100).WithMessage(_localizer["Username must be between 2 and 100 characters."]);
        RuleFor(p => p.Password)
            .NotEmpty().WithMessage(_localizer["Your password cannot be empty"])
            .MinimumLength(6).WithMessage(_localizer["Your password length must be at least 6 characters."])
            .MaximumLength(16).WithMessage(_localizer["Your password length must not exceed 16 characters."]);
    }

   
}