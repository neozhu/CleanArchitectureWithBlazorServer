#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Infrastructure.Services.MediatorWrapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Infrastructure.MediatorCompatibility;

public class ScopedMediatorTests
{
    [Test]
    public async Task Send_resolves_mediator_from_a_created_scope()
    {
        var scopeFactory = new RecordingScopeFactory();
        var sut = new ScopedMediator(scopeFactory);

        string result = await sut.Send(new PingQuery());

        Assert.That(result, Is.EqualTo("pong"));
        Assert.That(scopeFactory.CreateScopeCalls, Is.EqualTo(1));
        Assert.That(scopeFactory.Mediator.ResolvedRequests, Has.Count.EqualTo(1));
    }

    private sealed record PingQuery : Mediator.IRequest<string>;

    private sealed class RecordingScopeFactory : IServiceScopeFactory
    {
        private readonly RecordingMediator _mediator = new();

        public int CreateScopeCalls { get; private set; }

        public RecordingMediator Mediator => _mediator;

        public IServiceScope CreateScope()
        {
            CreateScopeCalls++;
            return new RecordingScope(_mediator);
        }
    }

    private sealed class RecordingScope : IServiceScope
    {
        public RecordingScope(RecordingMediator mediator)
        {
            ServiceProvider = new RecordingServiceProvider(mediator);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
        }
    }

    private sealed class RecordingServiceProvider : IServiceProvider
    {
        private readonly RecordingMediator _mediator;

        public RecordingServiceProvider(RecordingMediator mediator)
        {
            _mediator = mediator;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(Mediator.IMediator))
            {
                return _mediator;
            }

            return null;
        }
    }

    private sealed class RecordingMediator : Mediator.IMediator
    {
        public List<object> ResolvedRequests { get; } = new();

        public Task<TResponse> Send<TResponse>(Mediator.IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            ResolvedRequests.Add(request!);

            if (request is PingQuery)
            {
                return Task.FromResult((TResponse)(object)"pong");
            }

            throw new NotSupportedException($"Unexpected request type: {request.GetType().Name}");
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : Mediator.IRequest
        {
            throw new NotSupportedException();
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : Mediator.INotification
        {
            throw new NotSupportedException();
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            Mediator.IStreamRequest<TResponse> request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            yield break;
        }

        public async IAsyncEnumerable<object?> CreateStream(
            object request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
