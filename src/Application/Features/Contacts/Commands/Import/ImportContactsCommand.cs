﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under one or more agreements.
//     The .NET Foundation licenses this file to you under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2024-11-12
//     Last Modified: 2024-11-12
//     Description: 
//       This file defines the command, handler, and associated logic for importing 
//       contacts from an Excel file into the CleanArchitecture.Blazor application. 
//       The import process supports validating data and ensuring no duplicates are 
//       inserted. Additionally, a command for creating a contact template file is provided 
//       to facilitate bulk data entry for end users.
//     
//     Documentation:
//       https://docs.cleanarchitectureblazor.com/features/contact
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// - Use `ImportContactsCommand` to import contacts from an Excel file, ensuring proper validation
//   and avoiding duplicates.
// - Use `CreateContactsTemplateCommand` to generate an Excel template for entering contact data 
//   that can be later imported using the import command.

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Import;

    public class ImportContactsCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => ContactCacheKey.GetAllCacheKey;
        public IEnumerable<string>? Tags => ContactCacheKey.Tags;
        public ImportContactsCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateContactsTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportContactsCommandHandler : 
                 IRequestHandler<CreateContactsTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportContactsCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<ImportContactsCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly ContactDto _dto = new();

        public ImportContactsCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportContactsCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportContactsCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, ContactDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x=>x.Name)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Description)], (row, item) => item.Description = row[_localizer[_dto.GetMemberDescription(x=>x.Description)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Email)], (row, item) => item.Email = row[_localizer[_dto.GetMemberDescription(x=>x.Email)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.PhoneNumber)], (row, item) => item.PhoneNumber = row[_localizer[_dto.GetMemberDescription(x=>x.PhoneNumber)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Country)], (row, item) => item.Country = row[_localizer[_dto.GetMemberDescription(x=>x.Country)]].ToString() }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.Contacts.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                    if (!exists)
                    {
                        var item = ContactMapper.FromDto(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new ContactCreatedEvent(item));
                        await _context.Contacts.AddAsync(item, cancellationToken);
                    }
                 }
                 await _context.SaveChangesAsync(cancellationToken);
                 return await Result<int>.SuccessAsync(result.Data.Count());
           }
           else
           {
               return await Result<int>.FailureAsync(result.Errors);
           }
        }
        public async Task<Result<byte[]>> Handle(CreateContactsTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportContactsCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.Name)], 
_localizer[_dto.GetMemberDescription(x=>x.Description)], 
_localizer[_dto.GetMemberDescription(x=>x.Email)], 
_localizer[_dto.GetMemberDescription(x=>x.PhoneNumber)], 
_localizer[_dto.GetMemberDescription(x=>x.Country)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

