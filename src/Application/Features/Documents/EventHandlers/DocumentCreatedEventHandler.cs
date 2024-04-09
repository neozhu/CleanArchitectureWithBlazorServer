// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentCreatedEventHandler : INotificationHandler<CreatedEvent<Document>>
{
    private readonly ILogger<DocumentCreatedEventHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;


    public DocumentCreatedEventHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<DocumentCreatedEventHandler> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task Handle(CreatedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Document upload successful. Beginning document recognition process for Document Id: {DocumentId}",
            notification.Entity.Id);
        var domainEvent = notification.Entity;
        var id = domainEvent.Id;
        var ocrJob = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDocumentOcrJob>();
        BackgroundJob.Enqueue(() => ocrJob.Do(id));
        return Task.CompletedTask;
    }
}