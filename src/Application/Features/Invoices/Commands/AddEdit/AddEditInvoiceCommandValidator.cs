namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.AddEdit;

public class AddEditInvoiceCommandValidator : AbstractValidator<AddEditInvoiceCommand>
{
    public AddEditInvoiceCommandValidator()
    {
        RuleFor(v => v.InvoiceDate).NotNull();
        RuleFor(v => v.TotalAmount).NotNull();
        RuleFor(v => v.Status).MaximumLength(255);

    }

}

