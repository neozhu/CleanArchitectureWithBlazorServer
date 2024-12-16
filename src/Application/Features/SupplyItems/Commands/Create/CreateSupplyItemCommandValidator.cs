

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Create;

public class CreateSupplyItemCommandValidator : AbstractValidator<CreateSupplyItemCommand>
{
        public CreateSupplyItemCommandValidator()
        {
                RuleFor(v => v.ProductId).NotNull(); 
                RuleFor(v => v.SupplierId).NotNull(); 
                RuleFor(v => v.Quantity).NotNull(); 
                RuleFor(v => v.CostPerItem).NotNull(); 
                RuleFor(v => v.Notes).MaximumLength(255); 

        }
       
}

