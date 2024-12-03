
namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.AddEdit;

public class AddEditCategoryCommandValidator : AbstractValidator<AddEditCategoryCommand>
{
    public AddEditCategoryCommandValidator()
    {
                RuleFor(v => v.Name).MaximumLength(50).NotEmpty(); 

     }

}

