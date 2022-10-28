using CleanArchitecture.Blazor.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace Blazor.Server.UI.Pages.Identity.Users;

public class UserFormModel
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Provider { get; set; }
    public string? SuperiorId { get; set; }
    public string? Superior { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? PhoneNumber { get; set; }
    public string[]? AssignRoles { get; set; }
    public bool IsActive { get; set; }
    public bool Checked { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
    public bool IsLive { get; set; }
    public string? Role { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}
public class UserFormModelValidator : AbstractValidator<UserFormModel>
{
    public UserFormModelValidator()
    {
        RuleFor(v => v.TenantId)
             .MaximumLength(256)
             .NotEmpty();
        RuleFor(v => v.SuperiorId)
             .MaximumLength(256)
             .NotEmpty().When(x=>x.UserName!= "administrator");
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
        var result = await ValidateAsync(ValidationContext<UserFormModel>.CreateWithOptions((UserFormModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}