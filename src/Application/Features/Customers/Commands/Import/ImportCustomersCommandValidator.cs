// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.Import;

public class ImportCustomersCommandValidator : AbstractValidator<ImportCustomersCommand>
{
        public ImportCustomersCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

