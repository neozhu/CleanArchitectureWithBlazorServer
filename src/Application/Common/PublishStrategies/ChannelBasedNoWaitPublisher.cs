using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace CleanArchitecture.Blazor.Application.Common.PublishStrategies;

/// <summary>
/// High-performance publisher using Channel with backpressure control
/// </summary>
public class ChannelBasedNoWaitPublisher : INotificationPublisher
{
    private readonly ILogger<ChannelBasedNoWaitPublisher> _logger;
    private readonly Channel<(NotificationHandlerExecutor Handler, INotification Notification, CancellationToken CancellationToken)> _channel;
    private readonly ChannelWriter<(NotificationHandlerExecutor Handler, INotification Notification, CancellationToken CancellationToken)> _writer;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _shutdownTokenSource;

    public ChannelBasedNoWaitPublisher(ILogger<ChannelBasedNoWaitPublisher> logger, int capacity = 1000)
    {
        _logger = logger;
        _shutdownTokenSource = new CancellationTokenSource();
        
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        };

        _channel = Channel.CreateBounded<(NotificationHandlerExecutor, INotification, CancellationToken)>(options);
        _writer = _channel.Writer;

        // Start background processing task
        _processingTask = Task.Run(ProcessNotifications, _shutdownTokenSource.Token);
    }

    public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification,
        CancellationToken cancellationToken)
    {
        var handlers = handlerExecutors.ToList();
        
        if (!handlers.Any())
            return;

        // Add all handlers to channel for async processing
        foreach (var handler in handlers)
        {
            try
            {
                await _writer.WriteAsync((handler, notification, cancellationToken), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue handler for {NotificationType}", notification.GetType().Name);
            }
        }
    }

    private async Task ProcessNotifications()
    {
        await foreach (var (handler, notification, cancellationToken) in _channel.Reader.ReadAllAsync(_shutdownTokenSource.Token))
        {
            try
            {
                await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Handler execution was cancelled - this is expected during shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handler execution failed for {NotificationType}: {ErrorMessage}", 
                    notification.GetType().Name, ex.Message);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _writer.Complete();
        _shutdownTokenSource.Cancel();
        
        try
        {
            await _processingTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Expected cancellation exception
        }
        
        _shutdownTokenSource.Dispose();
    }
}
