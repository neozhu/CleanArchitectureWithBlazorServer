#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using Mediator;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Interceptors;

[TestFixture]
public class SaveChangesInterceptorRegressionTests
{
    [Test]
    public async Task DispatchDomainEventsInterceptor_ShouldPublishAndClearDomainEvents()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Publish(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        await using var context = await CreateContextAsync(new DispatchDomainEventsInterceptor(mediator.Object));

        var contact = new Contact { Name = "Regression Test Contact" };
        contact.AddDomainEvent(new ContactCreatedEvent(contact));

        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        mediator.Verify(x => x.Publish(
                It.Is<DomainEvent>(evt => evt.GetType() == typeof(ContactCreatedEvent) &&
                                          ((ContactCreatedEvent)evt).Item == contact),
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.That(contact.DomainEvents, Is.Empty);
    }

    [Test]
    public async Task DispatchDomainEventsInterceptor_ShouldPublishUpdatedContactDomainEvents()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Publish(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        await using var context = await CreateContextAsync(new DispatchDomainEventsInterceptor(mediator.Object));

        var contact = new Contact { Name = "Updated Contact" };
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        mediator.Reset();
        mediator.Setup(x => x.Publish(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        contact.Description = "Updated description";
        contact.AddDomainEvent(new ContactUpdatedEvent(contact));

        await context.SaveChangesAsync();

        mediator.Verify(x => x.Publish(
                It.Is<DomainEvent>(evt => evt.GetType() == typeof(ContactUpdatedEvent) &&
                                          ((ContactUpdatedEvent)evt).Item == contact),
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.That(contact.DomainEvents, Is.Empty);
    }

    [Test]
    public async Task DispatchDomainEventsInterceptor_ShouldPublishDeletedContactDomainEvents()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Publish(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        await using var context = await CreateContextAsync(new DispatchDomainEventsInterceptor(mediator.Object));

        var contact = new Contact { Name = "Deleted Contact" };
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        mediator.Reset();
        mediator.Setup(x => x.Publish(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        contact.AddDomainEvent(new ContactDeletedEvent(contact));
        context.Contacts.Remove(contact);

        await context.SaveChangesAsync();

        mediator.Verify(x => x.Publish(
                It.Is<DomainEvent>(evt => evt.GetType() == typeof(ContactDeletedEvent) &&
                                          ((ContactDeletedEvent)evt).Item == contact),
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.That(contact.DomainEvents, Is.Empty);
    }

    [Test]
    public async Task AuditableEntityInterceptor_ShouldPublishAuditTrailsReadyEventForAuditedEntities()
    {
        var mediator = new Mock<IMediator>();
        AuditTrailsReadyEvent? publishedEvent = null;
        mediator.Setup(x => x.Publish(It.IsAny<AuditTrailsReadyEvent>(), It.IsAny<CancellationToken>()))
            .Callback<AuditTrailsReadyEvent, CancellationToken>((notification, _) => publishedEvent = notification)
            .Returns(ValueTask.CompletedTask);

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.SetupGet(x => x.Current)
            .Returns(new UserContext("user-123", "regression-user"));

        var dateTime = new Mock<IDateTime>();
        dateTime.SetupGet(x => x.Now).Returns(new DateTime(2026, 3, 29, 9, 0, 0, DateTimeKind.Utc));

        await using var context = await CreateContextAsync(
            new AuditableEntityInterceptor(userContextAccessor.Object, dateTime.Object, mediator.Object));

        context.PicklistSets.Add(new PicklistSet
        {
            Name = Picklist.Brand,
            Value = "regression-value",
            Text = "Regression Value",
            Description = "audit regression"
        });

        await context.SaveChangesAsync();

        mediator.Verify(x => x.Publish(It.IsAny<AuditTrailsReadyEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.That(publishedEvent, Is.Not.Null);
        Assert.That(publishedEvent!.AuditTrails, Has.Count.EqualTo(1));
        var auditTrail = publishedEvent.AuditTrails.First();
        Assert.That(auditTrail.TableName, Is.EqualTo(nameof(PicklistSet)));
        Assert.That(auditTrail.UserId, Is.EqualTo("user-123"));
    }

    private static async Task<ApplicationDbContext> CreateContextAsync(params Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor[] interceptors)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .AddInterceptors(interceptors)
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }
}
