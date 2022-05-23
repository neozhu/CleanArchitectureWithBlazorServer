using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class MediatorExtensions
{
    public static async Task DispatchDomainEvents(this IMediator mediator, DbContext context)
    {
        var entities = context.ChangeTracker
            .Entries<IHasDomainEvent>()
            .Where(e => e.Entity.DomainEvents.Any(x=>!x.IsPublished))
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            domainEvent.IsPublished = true;
            await mediator.Publish(domainEvent);
        }
    }
}
