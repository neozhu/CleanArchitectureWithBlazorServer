// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentDeletedEventHandler : INotificationHandler<DeletedEvent<Document>>
{

    private readonly ILogger<DocumentCreatedEventHandler> _logger;


    public DocumentDeletedEventHandler(
        ILogger<DocumentCreatedEventHandler> logger

        )
    {
        _logger = logger;

    }
    public  Task Handle(DeletedEvent<Document> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete file: {fileName}", notification.Entity.URL);
        if (!string.IsNullOrEmpty(notification.Entity.URL))
        {
            var folder = UploadType.Document.GetDescription();
            var folderName = Path.Combine("Files", folder);
            var deletefile = Path.Combine(Directory.GetCurrentDirectory(), folderName, notification.Entity.URL);
            if (File.Exists(deletefile))
            {
                File.Delete(deletefile);
            }
        }
        return Task.CompletedTask;
    }
}
