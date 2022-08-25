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
        var entities = GetEntitiesWithPendingEvents(context);
        while (entities.Any())
        {
            foreach (var entity in entities)
                foreach (var domainEvent in entity.DomainEvents.ToList())
                {
                    await mediator.Publish(domainEvent);
                    entity.RemoveDomainEvent(domainEvent);
                }
            entities = GetEntitiesWithPendingEvents(context);
        }
    }
    public static async Task DispatchDeletedDomainEvents(this IMediator mediator, DbContext context)
    {
        var entities = GetEntitiesWithDeletingEvents(context);
        while (entities.Any())
        {
            foreach (var entity in entities)
                foreach (var domainEvent in entity.DomainEvents.ToList())
                {
                    await mediator.Publish(domainEvent);
                    entity.RemoveDomainEvent(domainEvent);
                }
            entities = GetEntitiesWithDeletingEvents(context);
        }
    }
    private static List<BaseEntity> GetEntitiesWithPendingEvents(DbContext context)
    {
        return context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any() && e.State != EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();
    }
    private static List<BaseEntity> GetEntitiesWithDeletingEvents(DbContext context)
    {
        return context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any() && e.State==EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();
    }
}