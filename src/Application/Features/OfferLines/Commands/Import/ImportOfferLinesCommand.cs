
using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Import;

    public class ImportOfferLinesCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => OfferLineCacheKey.GetAllCacheKey;
        public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
        public ImportOfferLinesCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateOfferLinesTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportOfferLinesCommandHandler : 
                 IRequestHandler<CreateOfferLinesTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportOfferLinesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<ImportOfferLinesCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly OfferLineDto _dto = new();

        public ImportOfferLinesCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportOfferLinesCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportOfferLinesCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, OfferLineDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.OfferId)], (row, item) => item.OfferId =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.OfferId)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.ItemId)], (row, item) => item.ItemId =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.ItemId)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Quantity)], (row, item) => item.Quantity =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.Quantity)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Discount)], (row, item) => item.Discount =Convert.ToDecimal(row[_localizer[_dto.GetMemberDescription(x=>x.Discount)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.LineTotal)], (row, item) => item.LineTotal =Convert.ToDecimal(row[_localizer[_dto.GetMemberDescription(x=>x.LineTotal)]]) }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.OfferLines.AnyAsync(x => x.Id == dto.Id, cancellationToken);
                    if (!exists)
                    {
                        var item = OfferLineMapper.FromDto(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new OfferLineCreatedEvent(item));
                        await _context.OfferLines.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateOfferLinesTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportOfferLinesCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.OfferId)], 
_localizer[_dto.GetMemberDescription(x=>x.ItemId)], 
_localizer[_dto.GetMemberDescription(x=>x.Quantity)], 
_localizer[_dto.GetMemberDescription(x=>x.Discount)], 
_localizer[_dto.GetMemberDescription(x=>x.LineTotal)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

