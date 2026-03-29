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

namespace Mediator
{
    public interface INotificationPublisher : global::MediatR.INotificationPublisher
    {
    }

    public class NotificationHandlerExecutor : global::MediatR.NotificationHandlerExecutor
    {
        public NotificationHandlerExecutor(Func<global::MediatR.INotification, CancellationToken, Task> handlerCallback)
            : base(handlerCallback)
        {
        }
    }
}
