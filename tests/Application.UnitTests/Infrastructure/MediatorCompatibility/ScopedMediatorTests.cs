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
        var services = new ServiceCollection();
        services.AddScoped<ScopeProbe>();
        services.AddScoped<Mediator.IMediator, FakeMediator>();

        using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        });
        var sut = new ScopedMediator(provider.GetRequiredService<IServiceScopeFactory>());

        string result = await sut.Send(new PingQuery());

        Assert.That(result, Is.EqualTo("pong"));
        Assert.That(FakeMediator.SendCallCount, Is.EqualTo(1));
        Assert.That(ScopeProbe.CreatedCount, Is.EqualTo(1));
        Assert.That(FakeMediator.ResolvedProbeIds, Has.Count.EqualTo(1));
        Assert.That(FakeMediator.ResolvedProbeIds[0], Is.Not.EqualTo(Guid.Empty));
    }

    private sealed record PingQuery : Mediator.IRequest<string>;

    private sealed class ScopeProbe
    {
        public static int CreatedCount { get; private set; }

        public Guid Id { get; } = Guid.NewGuid();

        public ScopeProbe()
        {
            CreatedCount++;
        }
    }

    private sealed class FakeMediator : Mediator.IMediator
    {
        public static int SendCallCount { get; private set; }

        public static List<Guid> ResolvedProbeIds { get; } = new();

        private readonly ScopeProbe _scopeProbe;

        public FakeMediator(ScopeProbe scopeProbe)
        {
            _scopeProbe = scopeProbe;
        }

        public Task<TResponse> Send<TResponse>(Mediator.IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            SendCallCount++;
            ResolvedProbeIds.Add(_scopeProbe.Id);

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
