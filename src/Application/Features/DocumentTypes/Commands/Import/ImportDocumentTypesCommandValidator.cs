// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.DocumentTypes.Commands.Import;

public class ImportDocumentTypesCommandValidator : AbstractValidator<ImportDocumentTypesCommand>
{
    public ImportDocumentTypesCommandValidator()
    {
        RuleFor(x => x.Data).NotNull().NotEmpty();
    }
}
