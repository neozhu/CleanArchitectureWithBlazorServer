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
            .NotEmpty()
            .Length(2, 100);
        RuleFor(p => p.Password).NotEmpty().WithMessage(_localizer["Your password cannot be empty"])
                  .MinimumLength(6).WithMessage(_localizer["Your password length must be at least 6."])
                  .MaximumLength(16).WithMessage(_localizer["Your password length must not exceed 16."])
                  .Matches(@"[A-Z]+").WithMessage(_localizer["Your password must contain at least one uppercase letter."])
                  .Matches(@"[a-z]+").WithMessage(_localizer["Your password must contain at least one lowercase letter."])
                  .Matches(@"[0-9]+").WithMessage(_localizer["Your password must contain at least one number."])
                  .Matches(@"[\@\!\?\*\.]+").WithMessage(_localizer["Your password must contain at least one (@!? *.)."]);

    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<LoginFormModel>.CreateWithOptions((LoginFormModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}