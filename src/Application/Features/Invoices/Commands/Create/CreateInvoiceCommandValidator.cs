namespace CleanArchitecture.Blazor.Application.Features.Invoices.Commands.Create;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(v => v.InvoiceDate).NotNull();
        RuleFor(v => v.TotalAmount).NotNull();
        RuleFor(v => v.Status).MaximumLength(255);

    }

}

