namespace MediatR
{
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}

namespace Mediator
{
    public interface IRequestHandler<in TRequest, TResponse> : global::MediatR.IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
    }

    public interface INotificationHandler<in TNotification> : global::MediatR.INotificationHandler<TNotification>
        where TNotification : INotification
    {
    }
}
