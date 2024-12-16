

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Import;

public class ImportSupplyItemsCommandValidator : AbstractValidator<ImportSupplyItemsCommand>
{
    public ImportSupplyItemsCommandValidator()
    {

        RuleFor(v => v.Data)
             .NotNull()
             .NotEmpty();

    }
}

