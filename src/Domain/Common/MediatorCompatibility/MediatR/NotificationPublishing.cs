namespace MediatR
{
    public interface INotificationPublisher
    {
        Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification,
            CancellationToken cancellationToken);
    }

    public class NotificationHandlerExecutor
    {
        public NotificationHandlerExecutor(Func<INotification, CancellationToken, Task> handlerCallback)
        {
            HandlerCallback = handlerCallback ?? throw new ArgumentNullException(nameof(handlerCallback));
        }

        public Func<INotification, CancellationToken, Task> HandlerCallback { get; }
    }
}
