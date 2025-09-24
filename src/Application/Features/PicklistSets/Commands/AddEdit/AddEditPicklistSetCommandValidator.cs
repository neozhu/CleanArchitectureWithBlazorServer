// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.AddEdit;

public class AddEditPicklistSetCommandValidator : AbstractValidator<AddEditPicklistSetCommand>
{
    public AddEditPicklistSetCommandValidator()
    {
        RuleFor(v => v.Name).NotNull();
        RuleFor(v => v.Text).MaximumLength(50).NotEmpty();
        RuleFor(v => v.Value).MaximumLength(100).NotEmpty();
    }
}
