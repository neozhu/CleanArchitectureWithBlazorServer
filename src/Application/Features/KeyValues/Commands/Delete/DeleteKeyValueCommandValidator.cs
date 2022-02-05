// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.KeyValues.Commands.Delete;

public class DeleteKeyValueCommandValidator : AbstractValidator<DeleteKeyValueCommand>
{
    public DeleteKeyValueCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEqual(0);
    }
}
public class DeleteCheckedKeyValuesCommandValidator : AbstractValidator<DeleteCheckedKeyValuesCommand>
{
    public DeleteCheckedKeyValuesCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
