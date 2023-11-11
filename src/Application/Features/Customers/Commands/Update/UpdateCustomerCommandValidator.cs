// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.Update;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
        public UpdateCustomerCommandValidator()
        {
           RuleFor(v => v.Id).NotNull();
           RuleFor(v => v.Name).MaximumLength(256).NotEmpty();
          
        }
    
}

