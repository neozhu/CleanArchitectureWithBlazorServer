

namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Update;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
        public UpdateCategoryCommandValidator()
        {
           RuleFor(v => v.Id).NotNull();
               RuleFor(v => v.Name).MaximumLength(50).NotEmpty(); 

          
        }
    
}

