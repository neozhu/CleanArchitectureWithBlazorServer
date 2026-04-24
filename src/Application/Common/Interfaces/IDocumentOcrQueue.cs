using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IDocumentOcrQueue
{
    ValueTask EnqueueAsync(DocumentOcrRequest request, CancellationToken cancellationToken);
    ValueTask<DocumentOcrRequest> DequeueAsync(CancellationToken cancellationToken);
}
