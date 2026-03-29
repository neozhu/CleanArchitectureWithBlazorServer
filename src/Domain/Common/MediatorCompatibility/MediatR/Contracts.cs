namespace MediatR
{
    public interface IBaseRequest
    {
    }

    public interface IRequest : IBaseRequest
    {
    }

    public interface IRequest<out TResponse> : IBaseRequest
    {
    }

    public interface INotification
    {
    }

    public interface IStreamRequest<out TResponse> : IBaseRequest
    {
    }

    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest;

        Task<object?> Send(object request, CancellationToken cancellationToken = default);

        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;

        Task Publish(object notification, CancellationToken cancellationToken = default);

        IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default);
    }
}
