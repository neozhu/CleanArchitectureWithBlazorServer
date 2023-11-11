// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentDeletedEventHandler : INotificationHandler<DeletedEvent<Document>>
{
    private readonly ILogger<DocumentDeletedEventHandler> _logger;

    public DocumentDeletedEventHandler(ILogger<DocumentDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DeletedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete file: {FileName}", notification.Entity.URL);
        if (string.IsNullOrEmpty(notification.Entity.URL)) return Task.CompletedTask;

        var folder = UploadType.Document.GetDescription();
        var folderName = Path.Combine("Files", folder);
        var deleteFile = Path.Combine(Directory.GetCurrentDirectory(), folderName, notification.Entity.URL);
        if (File.Exists(deleteFile)) File.Delete(deleteFile);

        return Task.CompletedTask;
    }
}