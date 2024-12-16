

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Update;

public class UpdateSupplyItemCommandValidator : AbstractValidator<UpdateSupplyItemCommand>
{
    public UpdateSupplyItemCommandValidator()
    {
        RuleFor(v => v.Id).NotNull();
        RuleFor(v => v.ProductId).NotNull();
        RuleFor(v => v.SupplierId).NotNull();
        RuleFor(v => v.Quantity).NotNull();
        RuleFor(v => v.CostPerItem).NotNull();
        RuleFor(v => v.Notes).MaximumLength(255);


    }

}

