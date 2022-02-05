// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.Documents.Commands.Delete;

public class DeleteDocumentCommandValidator : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEqual(0);
    }
}
public class DeleteCheckedDocumentsCommandValidator : AbstractValidator<DeleteCheckedDocumentsCommand>
{
    public DeleteCheckedDocumentsCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
