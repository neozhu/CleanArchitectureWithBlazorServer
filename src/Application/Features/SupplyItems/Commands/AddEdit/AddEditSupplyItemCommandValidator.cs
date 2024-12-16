
namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.AddEdit;

public class AddEditSupplyItemCommandValidator : AbstractValidator<AddEditSupplyItemCommand>
{
    public AddEditSupplyItemCommandValidator()
    {
        RuleFor(v => v.ProductId).NotNull();
        RuleFor(v => v.SupplierId).NotNull();
        RuleFor(v => v.Quantity).NotNull();
        RuleFor(v => v.CostPerItem).NotNull();
        RuleFor(v => v.Notes).MaximumLength(255);

    }

}

