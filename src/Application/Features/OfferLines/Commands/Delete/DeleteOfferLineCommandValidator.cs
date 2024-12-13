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
//       This file defines the validation rules for the DeleteOfferLineCommand used
//       to delete OfferLine entities within the CleanArchitecture.Blazor application. 
//       It ensures that the command has valid input, particularly verifying that the 
//       list of offerline IDs to delete is not null and contains only positive IDs.
//     
//     Documentation:
//       https://docs.cleanarchitectureblazor.com/features/offerline
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// This validator ensures that the DeleteOfferLineCommand is valid before attempting 
// to delete offerline records from the system. It verifies that the ID list is not 
// null and that all IDs are greater than zero.

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Delete;

public class DeleteOfferLineCommandValidator : AbstractValidator<DeleteOfferLineCommand>
{
        public DeleteOfferLineCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

