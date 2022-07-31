// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.$safeprojectname$.Features.Customers.DTOs;
using CleanArchitecture.Blazor.$safeprojectname$.Features.Customers.Caching;

namespace CleanArchitecture.Blazor.$safeprojectname$.Features.Customers.Commands.Import;

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
           //TODO:Implementing ImportCustomersCommandHandler method
           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, CustomerDto, object>>
            {
                //eg. { _localizer["Name"], (row,item) => item.Name = row[_localizer["Name"]]?.ToString() },

            }, _localizer["Customers"]);
            if (result.Succeeded)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.Customers.AnyAsync(x => x.Name == dto.Name);
                    if (!exists)
                    {
                        var item = _mapper.Map<Customer>(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new CreatedEvent<Customer>(item));
                        await _context.Customers.AddAsync(item, cancellationToken);
                    }
                 }
                 await _context.SaveChangesAsync(cancellationToken);
                 return Result.Success();
           }
           else
           {
               return Result.Failure(result.Errors);
           }
        }
        public async Task<byte[]> Handle(CreateCustomersTemplateCommand request, CancellationToken cancellationToken)
        {
            //TODO:Implementing ImportCustomersCommandHandler method 
            var fields = new string[] {
                   //TODO:Defines the title and order of the fields to be imported's template
                   //_localizer["Name"],
                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer["Customers"]);
            return result;
        }
    }

