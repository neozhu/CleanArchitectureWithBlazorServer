// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.Upload;

public class UploadDocumentCommand : ICacheInvalidatorRequest<Result<int>>
{
    public UploadDocumentCommand(List<UploadRequest> uploadRequests)
    {
        UploadRequests = uploadRequests;
    }
    public List<UploadRequest> UploadRequests { get; set; }
    public IEnumerable<string>? Tags => DocumentCacheKey.Tags;
}

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<int>>
{
  
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IFileUploadService _uploadService;

    public UploadDocumentCommandHandler(
       IApplicationDbContextFactory dbContextFactory,
        IFileUploadService uploadService
    )
    {
        _dbContextFactory = dbContextFactory;
        _uploadService = uploadService;
    }

    public async ValueTask<Result<int>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var list = new List<Document>();
        foreach (var uploadRequest in request.UploadRequests)
        {
            var fileName = uploadRequest.FileName;
            var uploadResult = await _uploadService.UploadAsync(uploadRequest);
            if (!uploadResult.Succeeded)
            {
                return await Result<int>.FailureAsync(uploadResult.ErrorMessage ?? "Failed to upload document");
            }
            var document = new Document
            {
                Title = fileName,
                URL = uploadResult.Data!.Url,
                Status = JobStatus.Queueing,
                IsPublic = true,
                DocumentType = DocumentType.Image
            };
            document.AddDomainEvent(new DocumentCreatedEvent(document));
            list.Add(document);
        }

        if (!list.Any()) return await Result<int>.SuccessAsync(0);
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        await db.Documents.AddRangeAsync(list, cancellationToken);
        var result = await db.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}
