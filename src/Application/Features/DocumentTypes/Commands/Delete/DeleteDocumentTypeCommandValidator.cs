// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.DocumentTypes.Commands.Delete;

public class DeleteDocumentTypeCommandValidator : AbstractValidator<DeleteDocumentTypeCommand>
{
    public DeleteDocumentTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEqual(0);
    }
}
public class DeleteCheckedDocumentTypesCommandValidator : AbstractValidator<DeleteCheckedDocumentTypesCommand>
{
    public DeleteCheckedDocumentTypesCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
