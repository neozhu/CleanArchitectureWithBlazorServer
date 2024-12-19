namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Update;

public class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceCommandValidator()
    {
        RuleFor(v => v.Id).NotNull();
        RuleFor(v => v.InvoiceDate).NotNull();
        RuleFor(v => v.TotalAmount).NotNull();
        RuleFor(v => v.Status).MaximumLength(255);


    }

}

