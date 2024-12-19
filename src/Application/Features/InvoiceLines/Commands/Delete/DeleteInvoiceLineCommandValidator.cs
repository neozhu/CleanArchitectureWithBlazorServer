
namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Delete;

public class DeleteInvoiceLineCommandValidator : AbstractValidator<DeleteInvoiceLineCommand>
{
        public DeleteInvoiceLineCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

