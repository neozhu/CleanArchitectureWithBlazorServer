

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Import;

public class ImportInvoiceLinesCommandValidator : AbstractValidator<ImportInvoiceLinesCommand>
{
        public ImportInvoiceLinesCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

