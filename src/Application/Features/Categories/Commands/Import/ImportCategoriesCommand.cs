
using CleanArchitecture.Blazor.Application.Features.Categories.DTOs;
using CleanArchitecture.Blazor.Application.Features.Categories.Caching;
using CleanArchitecture.Blazor.Application.Features.Categories.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Import;

    public class ImportCategoriesCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => CategoryCacheKey.GetAllCacheKey;
        public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
        public ImportCategoriesCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateCategoriesTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportCategoriesCommandHandler : 
                 IRequestHandler<CreateCategoriesTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportCategoriesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<ImportCategoriesCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly CategoryDto _dto = new();

        public ImportCategoriesCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportCategoriesCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportCategoriesCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, CategoryDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x=>x.Name)]].ToString() }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.Categories.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                    if (!exists)
                    {
                        var item = CategoryMapper.FromDto(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new CategoryCreatedEvent(item));
                        await _context.Categories.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateCategoriesTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportCategoriesCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.Name)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

