// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.DocumentTypes.DTOs;

namespace CleanArchitecture.Razor.Application.Features.DocumentTypes.Queries.Export;

public class ExportDocumentTypesQuery : IRequest<byte[]>
{
    public string filterRules { get; set; }
    public string sort { get; set; } = "Id";
    public string order { get; set; } = "desc";
}

public class ExportDocumentTypesQueryHandler :
     IRequestHandler<ExportDocumentTypesQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportDocumentTypesQueryHandler> _localizer;

    public ExportDocumentTypesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IExcelService excelService,
        IStringLocalizer<ExportDocumentTypesQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _excelService = excelService;
        _localizer = localizer;
    }
    public async Task<byte[]> Handle(ExportDocumentTypesQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<DocumentType>(request.filterRules);
        var data = await _context.DocumentTypes.Where(filters)
            .OrderBy($"{request.sort} {request.order}")
            .ProjectTo<DocumentTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<DocumentTypeDto, object>>()
            {
                    //{ _localizer["Id"], item => item.Id },
                    { _localizer["Name"], item => item.Name },
                    { _localizer["Description"], item => item.Description },

            }, _localizer["DocumentTypes"]
            );
        return result;
    }


}
