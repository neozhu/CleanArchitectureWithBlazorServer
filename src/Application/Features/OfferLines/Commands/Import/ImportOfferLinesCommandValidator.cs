
// Usage:
// This validator is used to ensure that an ImportOfferLinesCommand has valid input 
// before attempting to import offerline data. It checks that the Data property is not 
// null and is not empty, ensuring that the command has valid content for import.

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Import;

public class ImportOfferLinesCommandValidator : AbstractValidator<ImportOfferLinesCommand>
{
        public ImportOfferLinesCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

