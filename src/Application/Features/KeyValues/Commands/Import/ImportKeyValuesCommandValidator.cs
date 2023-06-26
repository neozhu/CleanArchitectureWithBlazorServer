// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Import;

public class ImportKeyValuesCommandValidator : AbstractValidator<ImportKeyValuesCommand>
{
    public ImportKeyValuesCommandValidator()
    {
        RuleFor(x => x.Data).NotNull().NotEmpty();
    }
}