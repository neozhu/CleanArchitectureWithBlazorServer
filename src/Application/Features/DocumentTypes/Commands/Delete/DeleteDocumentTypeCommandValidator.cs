// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.Delete;

public class DeleteDocumentTypeCommandValidator : AbstractValidator<DeleteDocumentTypeCommand>
{
    public DeleteDocumentTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
