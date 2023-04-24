// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.Import;

    public class ImportCustomersCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => CustomerCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
        public ImportCustomersCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateCustomersTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportCustomersCommandHandler : 
                 IRequestHandler<CreateCustomersTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportCustomersCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportCustomersCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly CustomerDto _dto = new();

        public ImportCustomersCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportCustomersCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
        {
           // TODO: Implement ImportCustomersCommandHandler method
           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, CustomerDto, object?>>
            {
                // TODO: Define the fields that should be read from Excel, for example:
                { _localizer[_dto.GetMemberDescription(x=>x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x=>x.Name)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Description)], (row, item) => item.Description = row[_localizer[_dto.GetMemberDescription(x=>x.Description)]].ToString() }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.Customers.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                    if (!exists)
                    {
                        var item = _mapper.Map<Customer>(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new CustomerCreatedEvent(item));
                        await _context.Customers.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateCustomersTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportCustomersCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.Name)], 
_localizer[_dto.GetMemberDescription(x=>x.Description)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

