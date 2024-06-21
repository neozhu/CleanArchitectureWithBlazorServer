// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.AddEdit;

public class AddEditCustomerCommandValidator : AbstractValidator<AddEditCustomerCommand>
{
    public AddEditCustomerCommandValidator()
    {
            RuleFor(v => v.Name)
                .MaximumLength(256)
                .NotEmpty();
       
     }

}

