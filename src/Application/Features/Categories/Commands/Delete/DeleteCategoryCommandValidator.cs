
namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
        public DeleteCategoryCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

