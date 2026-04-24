#nullable enable
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.Documents.EventHandlers;

[TestFixture]
public class DocumentCreatedEventHandlerTests
{
    [Test]
    public async Task Handle_ShouldEnqueueRecognition_WithCurrentUserId()
    {
        var queue = new Mock<IDocumentOcrQueue>();
        DocumentOcrRequest? capturedRequest = null;
        queue.Setup(x => x.EnqueueAsync(It.IsAny<DocumentOcrRequest>(), It.IsAny<CancellationToken>()))
            .Callback<DocumentOcrRequest, CancellationToken>((request, _) => capturedRequest = request)
            .Returns(ValueTask.CompletedTask);

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.SetupGet(x => x.Current)
            .Returns(new UserContext("user-123", "neo", TenantId: "tenant-1"));

        var handler = new DocumentCreatedEventHandler(
            queue.Object,
            userContextAccessor.Object,
            NullLogger<DocumentCreatedEventHandler>.Instance);

        await handler.Handle(
            new DocumentCreatedEvent(new Document { Id = 42, Title = "invoice.png" }),
            CancellationToken.None);

        Assert.That(capturedRequest, Is.EqualTo(new DocumentOcrRequest(42, "user-123", "neo", "tenant-1")));
    }
}
#nullable restore
