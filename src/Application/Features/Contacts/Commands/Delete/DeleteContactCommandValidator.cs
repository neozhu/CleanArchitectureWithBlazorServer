﻿//------------------------------------------------------------------------------
// <auto-generated>
// CleanArchitecture.Blazor - MIT Licensed.
// Author: neozhu
// Created/Modified: 2025-03-13
// Validator for DeleteContactCommand: ensures the ID list for contact is not null and contains only positive IDs.
// Docs: https://docs.cleanarchitectureblazor.com/features/contact
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// Validates DeleteContactCommand by checking that the ID list is non-null and all IDs are > 0.


namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Delete;

public class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
{
        public DeleteContactCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

