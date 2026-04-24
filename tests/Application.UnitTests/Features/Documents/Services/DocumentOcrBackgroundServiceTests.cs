#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Infrastructure.Services.Identity;
using CleanArchitecture.Blazor.Infrastructure.Services.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.Documents.Services;

[TestFixture]
public class DocumentOcrBackgroundServiceTests
{
    [Test]
    public async Task ExecuteAsync_ShouldRestoreUserContextBeforeRecognition()
    {
        var recognitionCompleted = new TaskCompletionSource<string?>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();
        services.AddSingleton<IDocumentOcrQueue, DocumentOcrQueue>();
        services.AddScoped<IDocumentOcrJob>(sp =>
            new CallbackDocumentOcrJob(
                async (_, _, cancellationToken) =>
                {
                    var currentUserId = sp.GetRequiredService<IUserContextAccessor>().Current?.UserId;
                    recognitionCompleted.TrySetResult(currentUserId);
                    await Task.CompletedTask.WaitAsync(cancellationToken);
                }));

        await using var serviceProvider = services.BuildServiceProvider();
        var queue = serviceProvider.GetRequiredService<IDocumentOcrQueue>();
        var worker = new DocumentOcrBackgroundService(
            queue,
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            serviceProvider.GetRequiredService<IUserContextAccessor>(),
            NullLogger<DocumentOcrBackgroundService>.Instance);

        await worker.StartAsync(CancellationToken.None);
        try
        {
            await queue.EnqueueAsync(new DocumentOcrRequest(7, "user-456", "neo", "tenant-1"), CancellationToken.None);

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var observedUserId = await recognitionCompleted.Task.WaitAsync(timeout.Token);

            Assert.That(observedUserId, Is.EqualTo("user-456"));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
            worker.Dispose();
        }
    }

    private sealed class CallbackDocumentOcrJob : IDocumentOcrJob
    {
        private readonly Func<int, string?, CancellationToken, Task> _callback;

        public CallbackDocumentOcrJob(Func<int, string?, CancellationToken, Task> callback)
        {
            _callback = callback;
        }

        public Task Recognition(int id, string? userName, CancellationToken cancellationToken)
        {
            return _callback(id, userName, cancellationToken);
        }
    }
}
#nullable restore
