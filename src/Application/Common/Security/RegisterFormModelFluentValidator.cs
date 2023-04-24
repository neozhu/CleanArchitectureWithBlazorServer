namespace CleanArchitecture.Blazor.Application.Common.Security;

public class RegisterFormModelFluentValidator : AbstractValidator<RegisterFormModel>
{
    private readonly IStringLocalizer<RegisterFormModelFluentValidator> _localizer;

    public RegisterFormModelFluentValidator(
        IStringLocalizer<RegisterFormModelFluentValidator> localizer
        )
    {
        _localizer = localizer;
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(2, 100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(255)
            .EmailAddress();
        RuleFor(p => p.Password).NotEmpty().WithMessage(_localizer["Your password cannot be empty"])
                  .MinimumLength(6).WithMessage(_localizer["Your password length must be at least 6."])
                  .MaximumLength(16).WithMessage(_localizer["Your password length must not exceed 16."])
                  .Matches(@"[A-Z]+").WithMessage(_localizer["Your password must contain at least one uppercase letter."])
                  .Matches(@"[a-z]+").WithMessage(_localizer["Your password must contain at least one lowercase letter."])
                  .Matches(@"[0-9]+").WithMessage(_localizer["Your password must contain at least one number."])
                  .Matches(@"[\@\!\?\*\.]+").WithMessage(_localizer["Your password must contain at least one (@!? *.)."]);
        RuleFor(x => x.ConfirmPassword)
             .Equal(x => x.Password);
        RuleFor(x => x.AgreeToTerms)
            .Equal(true);
        
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<RegisterFormModel>.CreateWithOptions((RegisterFormModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}

