// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentCreatedEventHandler : INotificationHandler<CreatedEvent<Document>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DocumentCreatedEventHandler> _logger;


    public DocumentCreatedEventHandler(
        IServiceScopeFactory scopeFactory,
        IApplicationDbContext context,
        ILogger<DocumentCreatedEventHandler> logger

        )
    {
        _scopeFactory = scopeFactory;
        _context = context;
        _logger = logger;

    }
    public Task Handle(CreatedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("begin recognition: {id}", notification.Entity.Id);
        var domainEvent = notification.Entity;
        var id = domainEvent.Id;
        IDocumentOcrJob _ocrJob = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDocumentOcrJob>();
        BackgroundJob.Enqueue(() => _ocrJob.Do(id));
        return Task.CompletedTask;
    }
}
