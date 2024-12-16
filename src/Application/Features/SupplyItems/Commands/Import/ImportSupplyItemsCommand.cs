
using CleanArchitecture.Blazor.Application.Features.SupplyItems.DTOs;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Import;

    public class ImportSupplyItemsCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => SupplyItemCacheKey.GetAllCacheKey;
        public IEnumerable<string>? Tags => SupplyItemCacheKey.Tags;
        public ImportSupplyItemsCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateSupplyItemsTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportSupplyItemsCommandHandler : 
                 IRequestHandler<CreateSupplyItemsTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportSupplyItemsCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<ImportSupplyItemsCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly SupplyItemDto _dto = new();

        public ImportSupplyItemsCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportSupplyItemsCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportSupplyItemsCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, SupplyItemDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.ProductId)], (row, item) => item.ProductId =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.ProductId)]]) }, 
                { _localizer[_dto.GetMemberDescription(x=>x.SupplierId)], (row, item) => item.SupplierId =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.SupplierId)]]) }, 
                { _localizer[_dto.GetMemberDescription(x=>x.Quantity)], (row, item) => item.Quantity =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.Quantity)]]) }, 
                { _localizer[_dto.GetMemberDescription(x=>x.CostPerItem)], (row, item) => item.CostPerItem =Convert.ToDecimal(row[_localizer[_dto.GetMemberDescription(x=>x.CostPerItem)]]) }, 
                { _localizer[_dto.GetMemberDescription(x=>x.Notes)], (row, item) => item.Notes = row[_localizer[_dto.GetMemberDescription(x=>x.Notes)]].ToString() }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
            //        var exists = await _context.SupplyItems.AnyAsync(x => x.Name == dto.Name, cancellationToken);
            //        if (!exists)
            //        {
            //            var item = SupplyItemMapper.FromDto(dto);
            //            // add create domain events if this entity implement the IHasDomainEvent interface
				        //// item.AddDomainEvent(new SupplyItemCreatedEvent(item));
            //            await _context.SupplyItems.AddAsync(item, cancellationToken);
            //        }
                 }
                 await _context.SaveChangesAsync(cancellationToken);
                 return await Result<int>.SuccessAsync(result.Data.Count());
           }
           else
           {
               return await Result<int>.FailureAsync(result.Errors);
           }
        }
        public async Task<Result<byte[]>> Handle(CreateSupplyItemsTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportSupplyItemsCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.ProductId)], 
_localizer[_dto.GetMemberDescription(x=>x.SupplierId)], 
_localizer[_dto.GetMemberDescription(x=>x.Quantity)], 
_localizer[_dto.GetMemberDescription(x=>x.CostPerItem)], 
_localizer[_dto.GetMemberDescription(x=>x.Notes)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

