

namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Create;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
        public CreateCategoryCommandValidator()
        {
                RuleFor(v => v.Name).MaximumLength(50).NotEmpty(); 

        }
       
}

