using FluentValidation;

namespace Blazor.Server.UI.Pages.Identity.Roles;

public class RoleFormModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}


public class RoleFormModelValidator : AbstractValidator<RoleFormModel>
{
    public RoleFormModelValidator()
    {
        RuleFor(v => v.Name)
             .MaximumLength(256)
             .NotEmpty();
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<RoleFormModel>.CreateWithOptions((RoleFormModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };

}
