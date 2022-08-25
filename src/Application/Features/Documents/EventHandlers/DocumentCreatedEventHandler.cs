// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.



using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using Hangfire;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentCreatedEventHandler : INotificationHandler<CreatedEvent<Document>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDocumentOcrJob _ocrJob;
    private readonly ILogger<DocumentCreatedEventHandler> _logger;


    public DocumentCreatedEventHandler(
        IApplicationDbContext context,
        IDocumentOcrJob ocrJob,
        ILogger<DocumentCreatedEventHandler> logger

        )
    {
        _context = context;
        _ocrJob = ocrJob;
        _logger = logger;

    }
    public async Task Handle(CreatedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("begin recognition: {id}", notification.Entity.Id);
        var domainEvent = notification.Entity;
        var id = domainEvent.Id;
        var item = await _context.Documents.FindAsync(new object[] { id }, cancellationToken);
        if (item is not null)
        {
            item.Status = JobStatus.Queueing;
            await _context.SaveChangesAsync(cancellationToken);
            BackgroundJob.Enqueue(() => _ocrJob.Recognition(domainEvent.Id, cancellationToken));
        }
    }
}
