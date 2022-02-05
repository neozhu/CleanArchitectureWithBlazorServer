// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommandValidator : AbstractValidator<AddEditDocumentCommand>
{
    public AddEditDocumentCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotNull()
            .MaximumLength(256)
            .NotEmpty();
        RuleFor(v => v.DocumentTypeId)
            .NotNull()
            .NotEqual(0);
        RuleFor(v => v.Description)
            .MaximumLength(256);
        RuleFor(v => v.UploadRequest)
            .NotNull()
            .When(x => x.Id <= 0);


    }
}
