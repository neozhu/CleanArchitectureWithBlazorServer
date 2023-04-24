using MediatR;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class MediatorExtensions
{
    public static async Task DispatchDomainEvents(this IMediator mediator, DbContext context,List<DomainEvent> deletedDomainEvents)
    {
        // If the delete domain events list has a value publish it first.
        if (deletedDomainEvents.Any())
        {
            foreach (var domainEvent in deletedDomainEvents)
                await mediator.Publish(domainEvent);
        }

        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
    
    
   
}