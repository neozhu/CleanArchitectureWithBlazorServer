// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Razor.Application.Features.Products.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Products.Commands.Import;

    public class ImportProductsCommand: IRequest<Result>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
    public class CreateProductsTemplateCommand : IRequest<byte[]>
    {
        public IEnumerable<string> Fields { get; set; }
        public string SheetName { get; set; }
    }

    public class ImportProductsCommandHandler : 
                 IRequestHandler<CreateProductsTemplateCommand, byte[]>,
                 IRequestHandler<ImportProductsCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportProductsCommandHandler> _localizer;
        private readonly IExcelService _excelService;

        public ImportProductsCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportProductsCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
        }
        public async Task<Result> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
        {
           //TODO:Implementing ImportProductsCommandHandler method
           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, ProductDto, object>>
            {
                //ex. { _localizer["Name"], (row,item) => item.Name = row[_localizer["Name"]]?.ToString() },

            }, _localizer["Products"]);
           throw new System.NotImplementedException();
        }
        public async Task<byte[]> Handle(CreateProductsTemplateCommand request, CancellationToken cancellationToken)
        {
            //TODO:Implementing ImportProductsCommandHandler method 
            var fields = new string[] {
                   //TODO:Defines the title and order of the fields to be imported's template
                   //_localizer["Name"],
                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer["Products"]);
            return result;
        }
    }

