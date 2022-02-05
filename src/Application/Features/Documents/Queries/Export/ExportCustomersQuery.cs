// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Documents.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Documents.Queries.Export;

public class ExportDocumentsQuery : IRequest<byte[]>
{
    public string filterRules { get; set; }
    public string sort { get; set; } = "Id";
    public string order { get; set; } = "desc";
}

public class ExportDocumentsQueryHandler :
     IRequestHandler<ExportDocumentsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportDocumentsQueryHandler> _localizer;

    public ExportDocumentsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IExcelService excelService,
        IStringLocalizer<ExportDocumentsQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _excelService = excelService;
        _localizer = localizer;
    }
    public async Task<byte[]> Handle(ExportDocumentsQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<Document>(request.filterRules);
        var data = await _context.Documents.Where(filters)
            .OrderBy($"{request.sort} {request.order}")
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<DocumentDto, object>>()
            {
                    //{ _localizer["Id"], item => item.Id },
                    { _localizer["Title"], item => item.Title },
                    { _localizer["Description"], item => item.Description },
                    { _localizer["URL"], item => item.URL },
                    { _localizer["Created By"], item => item.CreatedBy }
            }, _localizer["Documents"]
            );
        return result;
    }


}
