using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace Blazor.Server.UI.Pages.Identity.Users;

public class ApplicationUserDtoValidator : AbstractValidator<ApplicationUserDto>
{

    public ApplicationUserDtoValidator()
    {
        RuleFor(v => v.TenantId)
             .MaximumLength(256)
             .NotEmpty();
        RuleFor(v => v.SuperiorId)
             .MaximumLength(256)
             .NotEmpty().When(x=>!x.UserName.Equals("Administrator", StringComparison.CurrentCultureIgnoreCase));
        RuleFor(v => v.Provider)
             .MaximumLength(256)
             .NotEmpty();
        RuleFor(v => v.UserName)
             .MaximumLength(256)
             .NotEmpty();
        RuleFor(v => v.Email)
             .MaximumLength(256)
             .NotEmpty()
             .EmailAddress();
        RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
                 .MinimumLength(6).WithMessage("Your password length must be at least 6.")
                 .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                 .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                 .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                 .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                 .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        RuleFor(x => x.ConfirmPassword)
             .Equal(x => x.Password);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ApplicationUserDto>.CreateWithOptions((ApplicationUserDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}