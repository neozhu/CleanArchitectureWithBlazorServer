// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentDeletedEventHandler : INotificationHandler<DocumentDeletedEvent>
{
    private readonly ILogger<DocumentDeletedEventHandler> _logger;

    public DocumentDeletedEventHandler(ILogger<DocumentDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(DocumentDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.Item.URL))
        {
            _logger.LogWarning("The document URL is null or empty, skipping file deletion.");
            return ValueTask.CompletedTask;
        }

        var folder = UploadType.Document.GetDisplayName();
        var folderName = Path.Combine("Files", folder);
        var deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, notification.Item.URL);

        if (File.Exists(deleteFilePath))
        {
            try
            {
                File.Delete(deleteFilePath);
                _logger.LogInformation("File deleted successfully: {FilePath}", deleteFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {FilePath}", deleteFilePath);
            }
        }
        else
        {
            _logger.LogWarning("File not found for deletion: {FilePath}", deleteFilePath);
        }

        return ValueTask.CompletedTask;
    }
}
