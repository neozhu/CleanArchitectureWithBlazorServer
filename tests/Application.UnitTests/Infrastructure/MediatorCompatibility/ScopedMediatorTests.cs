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
    [SetUp]
    public void SetUp()
    {
        ScopeProbe.Reset();
        FakeMediator.Reset();
    }

    [Test]
    public async Task Send_creates_and_disposes_a_child_scope()
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

        string firstResult = await sut.Send(new PingQuery());
        string secondResult = await sut.Send(new PingQuery());

        Assert.That(firstResult, Is.EqualTo("pong"));
        Assert.That(secondResult, Is.EqualTo("pong"));
        Assert.That(FakeMediator.SendCallCount, Is.EqualTo(2));
        Assert.That(ScopeProbe.CreatedCount, Is.EqualTo(2));
        Assert.That(ScopeProbe.DisposedCount, Is.EqualTo(2));
        Assert.That(ScopeProbe.CreatedIds[0], Is.Not.EqualTo(ScopeProbe.CreatedIds[1]));
        Assert.That(ScopeProbe.DisposedIds, Is.EquivalentTo(ScopeProbe.CreatedIds));
        Assert.That(FakeMediator.ResolvedProbeIds, Is.EquivalentTo(ScopeProbe.CreatedIds));
    }

    private sealed record PingQuery : Mediator.IRequest<string>;

    private sealed class ScopeProbe : IDisposable
    {
        public static int CreatedCount { get; private set; }
        public static int DisposedCount { get; private set; }

        public static List<Guid> CreatedIds { get; } = new();
        public static List<Guid> DisposedIds { get; } = new();

        public Guid Id { get; } = Guid.NewGuid();

        public ScopeProbe()
        {
            CreatedCount++;
            CreatedIds.Add(Id);
        }

        public void Dispose()
        {
            DisposedCount++;
            DisposedIds.Add(Id);
        }

        public static void Reset()
        {
            CreatedCount = 0;
            DisposedCount = 0;
            CreatedIds.Clear();
            DisposedIds.Clear();
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

        public static void Reset()
        {
            SendCallCount = 0;
            ResolvedProbeIds.Clear();
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
