
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.DTOs;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
using CleanArchitecture.Blazor.Application.Features.InvoiceLines.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Commands.Import;

    public class ImportInvoiceLinesCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => InvoiceLineCacheKey.GetAllCacheKey;
        public IEnumerable<string>? Tags => InvoiceLineCacheKey.Tags;
        public ImportInvoiceLinesCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateInvoiceLinesTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportInvoiceLinesCommandHandler : 
                 IRequestHandler<CreateInvoiceLinesTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportInvoiceLinesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<ImportInvoiceLinesCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly InvoiceLineDto _dto = new();

        public ImportInvoiceLinesCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportInvoiceLinesCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportInvoiceLinesCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, InvoiceLineDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.InvoiceId)], (row, item) => item.InvoiceId =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.InvoiceId)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.ProductId)], (row, item) => item.ProductId =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.ProductId)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Quantity)], (row, item) => item.Quantity =Convert.ToInt32(row[_localizer[_dto.GetMemberDescription(x=>x.Quantity)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.UnitPrice)], (row, item) => item.UnitPrice =Convert.ToDecimal(row[_localizer[_dto.GetMemberDescription(x=>x.UnitPrice)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.LineTotal)], (row, item) => item.LineTotal =Convert.ToDecimal(row[_localizer[_dto.GetMemberDescription(x=>x.LineTotal)]]) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Discount)], (row, item) => item.Discount =Convert.ToDecimal(row[_localizer[_dto.GetMemberDescription(x=>x.Discount)]]) }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
            //        var exists = await _context.InvoiceLines.AnyAsync(x => x.Name == dto.Name, cancellationToken);
            //        if (!exists)
            //        {
            //            var item = InvoiceLineMapper.FromDto(dto);
            //            // add create domain events if this entity implement the IHasDomainEvent interface
				        //// item.AddDomainEvent(new InvoiceLineCreatedEvent(item));
            //            await _context.InvoiceLines.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateInvoiceLinesTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportInvoiceLinesCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.InvoiceId)], 
_localizer[_dto.GetMemberDescription(x=>x.ProductId)], 
_localizer[_dto.GetMemberDescription(x=>x.Quantity)], 
_localizer[_dto.GetMemberDescription(x=>x.UnitPrice)], 
_localizer[_dto.GetMemberDescription(x=>x.LineTotal)], 
_localizer[_dto.GetMemberDescription(x=>x.Discount)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

