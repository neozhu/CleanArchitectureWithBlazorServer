// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Import;

    public class ImportContactsCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => ContactCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => ContactCacheKey.GetOrCreateTokenSource();
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
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportContactsCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly ContactDto _dto = new();

        public ImportContactsCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportContactsCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
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
                        var item = _mapper.Map<Contact>(dto);
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

