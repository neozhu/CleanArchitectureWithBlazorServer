// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.KeyValues.Commands.SaveChanged;

public class SaveChangedKeyValuesCommandValidator : AbstractValidator<SaveChangedKeyValuesCommand>
{
    public SaveChangedKeyValuesCommandValidator()
    {
        RuleFor(v => v.Items)
              .NotNull()
              .NotEmpty();
        RuleForEach(v => v.Items)
            .ChildRules(x => x.RuleFor(x => x.Name).NotNull())
            .ChildRules(x => x.RuleFor(x => x.Value).NotNull());
    }
}
