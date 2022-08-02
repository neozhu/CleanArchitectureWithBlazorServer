// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.Import;

    public class ImportCustomersCommand: IRequest<Result>, ICacheInvalidator
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
    public record class CreateCustomersTemplateCommand : IRequest<byte[]>
    {
 
    }

    public class ImportCustomersCommandHandler : 
                 IRequestHandler<CreateCustomersTemplateCommand, byte[]>,
                 IRequestHandler<ImportCustomersCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportCustomersCommandHandler> _localizer;
        private readonly IExcelService _excelService;

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
        public async Task<Result> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, CustomerDto, object?>> 
            {
                { _localizer["Name"], (row,item) => item.Name = row[_localizer["Name"]]?.ToString() },
                { _localizer["Description"], (row,item) => item.Description = row[_localizer["Description"]]?.ToString() },
            }, _localizer["Customers"]);

            if (result.Succeeded && result.Data is not null) 
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.Customers.AnyAsync(x => x.Name == dto.Name, cancellationToken: cancellationToken);
                    if (!exists)
                    {
                        var item = _mapper.Map<Customer>(dto);
				        item.AddDomainEvent(new CreatedEvent<Customer>(item));
                        await _context.Customers.AddAsync(item, cancellationToken);
                    }
                 }
                 await _context.SaveChangesAsync(cancellationToken);
                 return await Result.SuccessAsync();
           }
           else
           {
               return await Result.FailureAsync(result.Errors);
           }
        }
        
        public async Task<byte[]> Handle(CreateCustomersTemplateCommand request, CancellationToken cancellationToken)
        {
            var fields = new string[] {
                   _localizer["Name"],
                   _localizer["Description"],
                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer["Customers"]);
            return result;
        }
    }

