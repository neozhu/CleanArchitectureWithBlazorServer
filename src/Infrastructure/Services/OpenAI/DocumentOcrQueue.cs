using System.Threading.Channels;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Infrastructure.Services.OpenAI;

public sealed class DocumentOcrQueue : IDocumentOcrQueue
{
    private readonly Channel<DocumentOcrRequest> _channel;

    public DocumentOcrQueue(int capacity = 100)
    {
        _channel = Channel.CreateBounded<DocumentOcrRequest>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
    }

    public ValueTask EnqueueAsync(DocumentOcrRequest request, CancellationToken cancellationToken)
    {
        return _channel.Writer.WriteAsync(request, cancellationToken);
    }

    public ValueTask<DocumentOcrRequest> DequeueAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}
