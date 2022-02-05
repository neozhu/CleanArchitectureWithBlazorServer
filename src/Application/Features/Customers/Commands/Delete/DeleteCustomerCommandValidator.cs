// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.Customers.Commands.Delete;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEqual(0);
    }
}
public class DeleteCheckedCustomersCommandValidator : AbstractValidator<DeleteCheckedCustomersCommand>
{
    public DeleteCheckedCustomersCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
