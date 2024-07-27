// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Import;

public class ImportContactsCommandValidator : AbstractValidator<ImportContactsCommand>
{
        public ImportContactsCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

