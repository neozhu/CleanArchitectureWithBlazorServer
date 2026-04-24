// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Display(Name = "Id")] public int Id { get; set; }
    [Display(Name ="Title")] public string? Title { get; set; }
    [Display(Name = "Description")] public string? Description { get; set; }
    [Display(Name = "Is Public")] public bool IsPublic { get; set; }
    [Display(Name = "URL")] public string? URL { get; set; }
    [Display(Name = "Document Type")] public DocumentType DocumentType { get; set; } = DocumentType.Document;
    [Display(Name = "Tenant Id")] public string? TenantId { get; set; }
    [Display(Name = "Tenant Name")] public string? TenantName { get; set; }
    [Display(Name = "Status")] public JobStatus Status { get; set; } = JobStatus.NotStart;
    [Display(Name = "Content")] public string? Content { get; set; }
    public UploadRequest? UploadRequest { get; set; }
    public IEnumerable<string>? Tags => DocumentCacheKey.Tags;}

public class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IObjectMapper _objectMapper;
    private readonly IStringLocalizer<AddEditDocumentCommandHandler> _localizer;

    public AddEditDocumentCommandHandler(
        IApplicationDbContextFactory dbContextFactory,
        IObjectMapper objectMapper,
        IStringLocalizer<AddEditDocumentCommandHandler> localizer
    )
    {
        _dbContextFactory = dbContextFactory;
        _objectMapper = objectMapper;
        _localizer = localizer;
    }

    public async ValueTask<Result<int>> Handle(AddEditDocumentCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        Document document;
        if (request.Id > 0)
        {
            var existingDocument = await db.Documents.FindAsync(request.Id, cancellationToken);
            if (existingDocument == null) return await Result<int>.FailureAsync(_localizer["Document Not Found!"]);
            document = _objectMapper.Map(request, existingDocument);
        }
        else
        {
            document = _objectMapper.Map<Document>(request);
            db.Documents.Add(document);
        }
        await db.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(document.Id);
    }
}
