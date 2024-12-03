
namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Import;

public class ImportCategoriesCommandValidator : AbstractValidator<ImportCategoriesCommand>
{
        public ImportCategoriesCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

