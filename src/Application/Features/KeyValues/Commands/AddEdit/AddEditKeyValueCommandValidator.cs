// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommandValidator : AbstractValidator<AddEditKeyValueCommand>
{
    public AddEditKeyValueCommandValidator()
    {
        RuleFor(v => v.Name).NotNull();
        RuleFor(v => v.Text).MaximumLength(256).NotEmpty();
        RuleFor(v => v.Value).MaximumLength(256).NotEmpty();

    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddEditKeyValueCommand>.CreateWithOptions((AddEditKeyValueCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
