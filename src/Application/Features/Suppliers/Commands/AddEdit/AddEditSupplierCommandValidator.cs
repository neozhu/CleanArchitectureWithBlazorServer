﻿

namespace CleanArchitecture.Blazor.Application.Features.Suppliers.Commands.AddEdit;

public class AddEditSupplierCommandValidator : AbstractValidator<AddEditSupplierCommand>
{
    public AddEditSupplierCommandValidator()
    {
        RuleFor(v => v.Name).MaximumLength(50).NotEmpty(); 
        RuleFor(v => v.Address).MaximumLength(255); 
        RuleFor(v => v.Phone).MaximumLength(255); 
        RuleFor(v => v.Email).MaximumLength(255); 
        RuleFor(v => v.VAT).MaximumLength(255); 
        RuleFor(v => v.Country).MaximumLength(255); 

     }

}

