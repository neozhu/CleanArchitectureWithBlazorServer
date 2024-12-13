﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under one or more agreements.
//     The .NET Foundation licenses this file to you under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-12-13
//     Last Modified: 2024-12-13
//     Description: 
//       This file defines the validation rules for the ImportOfferLinesCommand 
//       within the CleanArchitecture.Blazor application. It ensures that the 
//       command's required properties are correctly set before proceeding with 
//       the offerline import process.
//     
//     Documentation:
//       https://docs.cleanarchitectureblazor.com/features/offerline
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// This validator is used to ensure that an ImportOfferLinesCommand has valid input 
// before attempting to import offerline data. It checks that the Data property is not 
// null and is not empty, ensuring that the command has valid content for import.

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Import;

public class ImportOfferLinesCommandValidator : AbstractValidator<ImportOfferLinesCommand>
{
        public ImportOfferLinesCommandValidator()
        {
           
           RuleFor(v => v.Data)
                .NotNull()
                .NotEmpty();

        }
}

