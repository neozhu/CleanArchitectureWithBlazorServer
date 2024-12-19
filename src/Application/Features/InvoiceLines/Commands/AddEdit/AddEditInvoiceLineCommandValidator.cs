
namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.AddEdit;

public class AddEditInvoiceLineCommandValidator : AbstractValidator<AddEditInvoiceLineCommand>
{
    public AddEditInvoiceLineCommandValidator()
    {
                RuleFor(v => v.InvoiceId).NotNull(); 
    RuleFor(v => v.ProductId).NotNull(); 
    RuleFor(v => v.Quantity).NotNull(); 
    RuleFor(v => v.UnitPrice).NotNull(); 
    RuleFor(v => v.LineTotal).NotNull(); 
    RuleFor(v => v.Discount).NotNull(); 

     }

}

