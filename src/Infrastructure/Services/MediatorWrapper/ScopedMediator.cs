using System.Runtime.CompilerServices;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MediatorWrapper;
using MediatR;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MediatorWrapper;

/// <summary>
/// Represents a scoped mediator that provides methods for sending requests, publishing notifications, and creating streams.
/// </summary>
public class ScopedMediator : IScopedMediator
{
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedMediator"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    public ScopedMediator(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            TResponse response = await mediator.Send(request, cancellationToken);

            return response;
        }
    }

    /// <inheritdoc/>
    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(request, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            object? response = await mediator.Send(request, cancellationToken);

            return response;
        }
    }

    /// <inheritdoc/>
    public async Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            IAsyncEnumerable<TResponse> items = mediator.CreateStream(request, cancellationToken);

            await foreach (TResponse item in items)
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<object?> CreateStream(
        object request,
        [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            IAsyncEnumerable<object?> items = mediator.CreateStream(request, cancellationToken);

            await foreach (object? item in items)
            {
                yield return item;
            }
        }
    }
}
