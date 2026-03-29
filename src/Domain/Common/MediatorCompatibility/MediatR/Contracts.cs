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

    public sealed class MediatRServiceConfiguration
    {
        public INotificationPublisher? NotificationPublisher { get; set; }

        public void RegisterServicesFromAssembly(System.Reflection.Assembly assembly)
        {
        }

        public void AddRequestPreProcessor(Type serviceType, Type implementationType)
        {
        }

        public void AddOpenBehavior(Type behaviorType)
        {
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddMediatR(
            this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,
            Action<MediatRServiceConfiguration> configuration)
        {
            configuration(new MediatRServiceConfiguration());
            return services;
        }
    }
}

namespace Mediator
{
    public interface IBaseRequest : global::MediatR.IBaseRequest
    {
    }

    public interface IRequest : global::MediatR.IRequest
    {
    }

    public interface IRequest<out TResponse> : global::MediatR.IRequest<TResponse>
    {
    }

    public interface INotification : global::MediatR.INotification
    {
    }

    public interface IStreamRequest<out TResponse> : global::MediatR.IStreamRequest<TResponse>
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
