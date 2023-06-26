// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete;

public class DeleteKeyValueCommandValidator : AbstractValidator<DeleteKeyValueCommand>
{
    public DeleteKeyValueCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}