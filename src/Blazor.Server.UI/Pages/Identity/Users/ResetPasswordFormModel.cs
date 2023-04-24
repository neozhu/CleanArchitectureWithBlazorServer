namespace Blazor.Server.UI.Pages.Identity.Users;

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
    public ResetPasswordFormModelValidator()
    {
        RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
                 .MinimumLength(6).WithMessage("Your password length must be at least 6.")
                 .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                 .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                 .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                 .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                 .Matches(@"[\@\!\?\*\.]+").WithMessage("Your password must contain at least one (@!? *.).");
        RuleFor(x => x.ConfirmPassword)
             .Equal(x => x.Password);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ResetPasswordFormModel>.CreateWithOptions((ResetPasswordFormModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
