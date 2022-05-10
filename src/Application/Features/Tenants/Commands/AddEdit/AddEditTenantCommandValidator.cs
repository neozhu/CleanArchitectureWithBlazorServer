// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommandValidator : AbstractValidator<AddEditTenantCommand>
{
    public AddEditTenantCommandValidator()
    {

       RuleFor(v => v.Name)
              .MaximumLength(256)
              .NotEmpty();

    }
     public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
     {
        var result = await ValidateAsync(ValidationContext<AddEditTenantCommand>.CreateWithOptions((AddEditTenantCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
     };
}

