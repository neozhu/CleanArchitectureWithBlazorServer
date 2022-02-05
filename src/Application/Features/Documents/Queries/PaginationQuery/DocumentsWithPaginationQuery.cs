// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Documents.DTOs;
using CleanArchitecture.Razor.Application.Common.Specification;

namespace CleanArchitecture.Razor.Application.Features.Documents.Queries.PaginationQuery;

public class DocumentsWithPaginationQuery : PaginationRequest, IRequest<PaginatedData<DocumentDto>>
{


}
public class DocumentsQueryHandler : IRequestHandler<DocumentsWithPaginationQuery, PaginatedData<DocumentDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DocumentsQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _currentUserService = currentUserService;
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedData<DocumentDto>> Handle(DocumentsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<Document>(request.FilterRules);

        var data = await _context.Documents
            .Specify(new DocumentsQuery(_currentUserService.UserId))
            .Where(filters)
            .OrderBy($"{request.Sort} {request.Order}")
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.Page, request.Rows);

        return data;
    }

    internal class DocumentsQuery : Specification<Document>
    {
        public DocumentsQuery(string userId)
        {
            this.AddInclude(x => x.DocumentType);
            this.Criteria = p => (p.CreatedBy == userId && p.IsPublic == false) || p.IsPublic == true;
        }
    }
}
