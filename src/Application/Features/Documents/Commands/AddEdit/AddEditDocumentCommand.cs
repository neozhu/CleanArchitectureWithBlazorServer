// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Documents.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommand : DocumentDto, IRequest<Result<int>>, IMapFrom<Document>
{
    public new void Mapping(Profile profile)
    {
        profile.CreateMap<AddEditDocumentCommand, Document>(MemberList.None);

    }
    public UploadRequest UploadRequest { get; set; }
}

public class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUploadService _uploadService;

    public AddEditDocumentCommandHandler(
        IApplicationDbContext context,
         IMapper mapper,
         IUploadService uploadService
        )
    {
        _context = context;
        _mapper = mapper;
        _uploadService = uploadService;
    }
    public async Task<Result<int>> Handle(AddEditDocumentCommand request, CancellationToken cancellationToken)
    {

        if (request.Id > 0)
        {
            var document = await _context.Documents.FindAsync(new object[] { request.Id }, cancellationToken);
            document.Title = request.Title;
            document.Description = request.Description;
            document.IsPublic = request.IsPublic;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(document.Id);
        }
        else
        {
            var result = await _uploadService.UploadAsync(request.UploadRequest);
            var document = _mapper.Map<Document>(request);
            document.URL = result;
            var createdevent = new DocumentCreatedEvent(document);
            document.DomainEvents.Add(createdevent);
            _context.Documents.Add(document);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(document.Id);
        }


    }
}
