// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.$safeprojectname$.Features.Documents.DTOs;
using CleanArchitecture.Blazor.$safeprojectname$.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.$safeprojectname$.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommand : DocumentDto, IRequest<Result<int>>, ICacheInvalidator
{
    public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.SharedExpiryTokenSource();
    public UploadRequest? UploadRequest { get; set; }
  
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
            _ = document ?? throw new NotFoundException($"Document {request.Id} Not Found.");
            document.AddDomainEvent(new UpdatedEvent<Document>(document));
            if (request.UploadRequest != null)
            {
                document.URL = await _uploadService.UploadAsync(request.UploadRequest);
            }
            document.Title = request.Title;
            document.Description = request.Description;
            document.IsPublic = request.IsPublic;
            document.DocumentType = request.DocumentType;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(document.Id);
        }
        else
        {
            var document = _mapper.Map<Document>(request);
            if (request.UploadRequest != null)
            {
                document.URL = await _uploadService.UploadAsync(request.UploadRequest); ;
            }
             document.AddDomainEvent(new CreatedEvent<Document>(document));
            _context.Documents.Add(document);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(document.Id);
        }


    }
}
