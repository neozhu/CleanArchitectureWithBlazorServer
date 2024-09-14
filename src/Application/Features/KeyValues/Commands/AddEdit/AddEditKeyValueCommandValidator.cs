// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommandValidator : AbstractValidator<AddEditKeyValueCommand>
{
    public AddEditKeyValueCommandValidator()
    {
        RuleFor(v => v.Name).NotNull();
        RuleFor(v => v.Text).MaximumLength(50).NotEmpty();
        RuleFor(v => v.Value).MaximumLength(100).NotEmpty();
    }
}