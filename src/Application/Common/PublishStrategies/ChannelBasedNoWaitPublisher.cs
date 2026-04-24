using System.Threading.Channels;

namespace CleanArchitecture.Blazor.Application.Common.PublishStrategies;

/// <summary>
/// High-performance publisher using Channel with backpressure control
/// </summary>
public class ChannelBasedNoWaitPublisher : INotificationPublisher
{
    private readonly ILogger<ChannelBasedNoWaitPublisher> _logger;
    private readonly Channel<(Func<CancellationToken, ValueTask> Callback, string NotificationType)> _channel;
    private readonly ChannelWriter<(Func<CancellationToken, ValueTask> Callback, string NotificationType)> _writer;
    private readonly Task _processingTask;
    private int _disposeState;

    public ChannelBasedNoWaitPublisher(ILogger<ChannelBasedNoWaitPublisher> logger, int capacity = 1000)
    {
        _logger = logger;
        
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        };

        _channel = Channel.CreateBounded<(Func<CancellationToken, ValueTask>, string)>(options);
        _writer = _channel.Writer;

        // Start background processing task
        _processingTask = Task.Run(ProcessNotifications);
    }

    public async ValueTask Publish<TNotification>(NotificationHandlers<TNotification> handlers, TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var handlerList = handlers.ToList();
        
        if (!handlerList.Any())
            return;

        // Add all handlers to channel for async processing
        foreach (var handler in handlerList)
        {
            try
            {
                await _writer.WriteAsync(
                    (token => handler.Handle(notification, token), notification.GetType().Name),
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue handler for {NotificationType}", notification.GetType().Name);
            }
        }
    }

    private async Task ProcessNotifications()
    {
        await foreach (var (callback, notificationType) in _channel.Reader.ReadAllAsync())
        {
            try
            {
                await callback(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handler execution failed for {NotificationType}: {ErrorMessage}", 
                    notificationType, ex.Message);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposeState, 1) != 0)
        {
            return;
        }

        _writer.TryComplete();

        try
        {
            await _processingTask.ConfigureAwait(false);
        }
        catch (ChannelClosedException)
        {
            // The reader observed a normal channel shutdown while draining.
        }
    }
}
