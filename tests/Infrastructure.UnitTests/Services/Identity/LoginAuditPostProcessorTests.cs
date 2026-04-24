using System.Collections.Concurrent;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Services.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CleanArchitecture.Blazor.Infrastructure.UnitTests.Services.Identity;

public class LoginAuditPostProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ShouldCreateFreshScopeAndDelegateToScopedServices()
    {
        var recorder = new CallRecorder();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(recorder);
        services.AddScoped<ScopeMarker>();
        services.AddScoped<ILoginAuditEnricher, TrackingLoginAuditEnricher>();
        services.AddScoped<ISecurityAnalysisService, TrackingSecurityAnalysisService>();
        services.AddSingleton<ILoginAuditPostProcessor, LoginAuditPostProcessor>();

        await using var provider = services.BuildServiceProvider();

        ILoginAuditPostProcessor processor;
        await using (var scope = provider.CreateAsyncScope())
        {
            processor = scope.ServiceProvider.GetRequiredService<ILoginAuditPostProcessor>();
        }

        await processor.ProcessAsync(new LoginAudit
        {
            Id = 7,
            UserId = "user-1",
            UserName = "neo",
            IpAddress = "8.8.8.8",
            Success = true
        });

        Assert.Single(recorder.EnricherScopeIds);
        Assert.Single(recorder.SecurityScopeIds);
        Assert.Equal(recorder.EnricherScopeIds.Single(), recorder.SecurityScopeIds.Single());
    }

    private sealed class ScopeMarker
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    private sealed class CallRecorder
    {
        public ConcurrentBag<Guid> EnricherScopeIds { get; } = [];
        public ConcurrentBag<Guid> SecurityScopeIds { get; } = [];
    }

    private sealed class TrackingLoginAuditEnricher : ILoginAuditEnricher
    {
        private readonly ScopeMarker _scopeMarker;
        private readonly CallRecorder _recorder;

        public TrackingLoginAuditEnricher(ScopeMarker scopeMarker, CallRecorder recorder)
        {
            _scopeMarker = scopeMarker;
            _recorder = recorder;
        }

        public Task EnrichAsync(LoginAudit loginAudit, CancellationToken cancellationToken = default)
        {
            _recorder.EnricherScopeIds.Add(_scopeMarker.Id);
            return Task.CompletedTask;
        }
    }

    private sealed class TrackingSecurityAnalysisService : ISecurityAnalysisService
    {
        private readonly ScopeMarker _scopeMarker;
        private readonly CallRecorder _recorder;

        public TrackingSecurityAnalysisService(ScopeMarker scopeMarker, CallRecorder recorder)
        {
            _scopeMarker = scopeMarker;
            _recorder = recorder;
        }

        public Task AnalyzeUserSecurityAsync(LoginAudit loginAudit, CancellationToken cancellationToken = default)
        {
            _recorder.SecurityScopeIds.Add(_scopeMarker.Id);
            return Task.CompletedTask;
        }
    }
}
