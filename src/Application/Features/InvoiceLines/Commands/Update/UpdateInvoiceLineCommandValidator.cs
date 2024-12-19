

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Update;

public class UpdateInvoiceLineCommandValidator : AbstractValidator<UpdateInvoiceLineCommand>
{
        public UpdateInvoiceLineCommandValidator()
        {
           RuleFor(v => v.Id).NotNull();
               RuleFor(v => v.InvoiceId).NotNull(); 
    RuleFor(v => v.ProductId).NotNull(); 
    RuleFor(v => v.Quantity).NotNull(); 
    RuleFor(v => v.UnitPrice).NotNull(); 
    RuleFor(v => v.LineTotal).NotNull(); 
    RuleFor(v => v.Discount).NotNull(); 

          
        }
    
}

