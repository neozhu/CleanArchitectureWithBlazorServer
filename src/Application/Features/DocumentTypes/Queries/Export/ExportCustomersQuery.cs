// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.DocumentTypes.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Queries.Export;

public class ExportDocumentTypesQuery : IRequest<byte[]>
{
    public string Keyword { get; set; }
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "desc";
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
       var data = await _context.DocumentTypes.Where(x=>x.Name.Contains(request.Keyword) || x.Description.Contains(request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo<DocumentTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<DocumentTypeDto, object>>()
            {
                    { _localizer["Name"], item => item.Name },
                    { _localizer["Description"], item => item.Description },

            }, _localizer["DocumentTypes"]
            );
        return result;
    }


}
