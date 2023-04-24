using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace Blazor.Server.UI.Pages.Identity.Roles;




public class RoleDtoValidator : AbstractValidator<ApplicationRoleDto>
{
    public RoleDtoValidator()
    {
        RuleFor(v => v.Name)
             .MaximumLength(256)
             .NotEmpty();
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ApplicationRoleDto>.CreateWithOptions((ApplicationRoleDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };

}
