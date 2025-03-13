﻿//------------------------------------------------------------------------------
// <auto-generated>
// CleanArchitecture.Blazor - MIT Licensed.
// Author: neozhu
// Created/Modified: 2025-03-13
// Validator for UpdateContactCommand: ensures required fields (e.g., Id, non-empty Name, max length for properties) are valid for updating a contact.
// Docs: https://docs.cleanarchitectureblazor.com/features/contact
// </auto-generated>
//------------------------------------------------------------------------------
//
// Usage:
// Validates UpdateContactCommand to ensure complete and valid data before processing the update.


namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Update;

public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
        public UpdateContactCommandValidator()
        {
           RuleFor(v => v.Id).NotNull();
               RuleFor(v => v.Name).MaximumLength(50).NotEmpty(); 
    RuleFor(v => v.Description).MaximumLength(255); 
    RuleFor(v => v.Email).MaximumLength(255); 
    RuleFor(v => v.PhoneNumber).MaximumLength(255); 
    RuleFor(v => v.Country).MaximumLength(255); 

          
        }
    
}

