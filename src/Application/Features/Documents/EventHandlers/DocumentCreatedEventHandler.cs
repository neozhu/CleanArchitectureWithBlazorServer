// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentCreatedEventHandler : INotificationHandler<DocumentCreatedEvent>
{
    private readonly ILogger<DocumentCreatedEventHandler> _logger;
    private readonly IDocumentOcrQueue _documentOcrQueue;
    private readonly IUserContextAccessor _userContextAccessor;


    public DocumentCreatedEventHandler(
        IDocumentOcrQueue documentOcrQueue,
        IUserContextAccessor userContextAccessor,
        ILogger<DocumentCreatedEventHandler> logger
    )
    {
        _documentOcrQueue = documentOcrQueue;
        _userContextAccessor = userContextAccessor;
        _logger = logger;
    }

    public async ValueTask Handle(DocumentCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Document upload successful. Document visual analysis for Document Id: {DocumentId},User: {@UserName}",
            notification.Item.Id,
            _userContextAccessor.Current?.UserName);

        await _documentOcrQueue.EnqueueAsync(
            new DocumentOcrRequest(notification.Item.Id, _userContextAccessor.Current?.UserId, _userContextAccessor.Current?.UserName, _userContextAccessor.Current?.TenantId),
            cancellationToken);
    }
}
