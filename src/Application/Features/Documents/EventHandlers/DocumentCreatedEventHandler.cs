// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.



namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentCreatedEventHandler : INotificationHandler<CreatedEvent<Document>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DocumentCreatedEventHandler> _logger;


    public DocumentCreatedEventHandler(
        IApplicationDbContext context,
        ILogger<DocumentCreatedEventHandler> logger

        )
    {
        _context = context;
        _logger = logger;

    }
    public Task Handle(CreatedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
