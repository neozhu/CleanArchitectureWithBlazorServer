

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Delete;

public class DeleteSupplyItemCommandValidator : AbstractValidator<DeleteSupplyItemCommand>
{
        public DeleteSupplyItemCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

