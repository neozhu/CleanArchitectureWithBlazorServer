using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly IDateTime _dateTime;
    private List<AuditTrail> _temporaryAuditTrailList = new();
    private List<DomainEvent> _deletingDomainEvents = new();
    public AuditableEntitySaveChangesInterceptor(
        ITenantProvider tenantProvider,
        ICurrentUserService currentUserService,
                IMediator mediator,
        IDateTime dateTime)
    {
        _tenantProvider = tenantProvider;
        _currentUserService = currentUserService;
        _mediator = mediator;
        _dateTime = dateTime;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {

        UpdateEntities(eventData.Context!);
        _temporaryAuditTrailList = TryInsertTemporaryAuditTrail(eventData.Context!, cancellationToken);
        _deletingDomainEvents = TryGetDeletingDomainEvents(eventData.Context!, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        var resultValueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        await TryUpdateTemporaryPropertiesForAuditTrail(eventData.Context!, cancellationToken).ConfigureAwait(false);
        await _mediator.DispatchDomainEvents(eventData.Context!, _deletingDomainEvents).ConfigureAwait(false);
        return resultValueTask;
    }
    private void UpdateEntities(DbContext context)
    {
        var userId = _currentUserService.UserId;
        var tenantId = _tenantProvider.TenantId;
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created = _dateTime.Now;
                    if (entry.Entity is IMustHaveTenant mustTenant)
                    {
                        mustTenant.TenantId = tenantId;
                    }
                    if (entry.Entity is IMayHaveTenant mayTenant)
                    {
                        mayTenant.TenantId = tenantId;
                    }
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = userId;
                        softDelete.Deleted = _dateTime.Now;
                        entry.State = EntityState.Modified;
                    }
                    break;
                case EntityState.Unchanged:
                    if (entry.HasChangedOwnedEntities())
                    {
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModified = _dateTime.Now;
                    }
                    break;
            }
        }
    }

    private List<AuditTrail> TryInsertTemporaryAuditTrail(DbContext context, CancellationToken cancellationToken = default)
    {

        var userId = _currentUserService.UserId;
        var tenantId = _tenantProvider.TenantId;
        context.ChangeTracker.DetectChanges();
        var temporaryAuditEntries = new List<AuditTrail>();
        foreach (var entry in context.ChangeTracker.Entries<IAuditTrial>())
        {
            if (entry.Entity is AuditTrail ||
                entry.State == EntityState.Detached ||
                entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditTrail()
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId,
                DateTime = _dateTime.Now,
                AffectedColumns = new List<string>(),
                NewValues = new(),
                OldValues = new(),
            };
            foreach (var property in entry.Properties)
            {

                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey() && property.CurrentValue is not null)
                {
                    auditEntry.PrimaryKey[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        if (property.CurrentValue is not null)
                        {
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        if (property.OriginalValue is not null)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                        }
                        break;

                    case EntityState.Modified when property.IsModified && ((property.OriginalValue is null && property.CurrentValue is not null) || (property.OriginalValue is not null && property.OriginalValue.Equals(property.CurrentValue) == false)):
                        auditEntry.AffectedColumns.Add(propertyName);
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        if (property.CurrentValue is not null)
                        {
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;

                }
            }
            temporaryAuditEntries.Add(auditEntry);
        }
        return temporaryAuditEntries;
    }

    private async Task TryUpdateTemporaryPropertiesForAuditTrail(DbContext context, CancellationToken cancellationToken = default)
    {
        if (_temporaryAuditTrailList.Any())
        {
            foreach (var auditEntry in _temporaryAuditTrailList)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue is not null)
                    {
                        auditEntry.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (auditEntry.NewValues is not null && prop.CurrentValue is not null)
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                
            }
            await context.AddRangeAsync(_temporaryAuditTrailList);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _temporaryAuditTrailList.Clear();
        }
    }

    private List<DomainEvent> TryGetDeletingDomainEvents(DbContext context, CancellationToken cancellationToken = default)
    {
        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any() && e.State == EntityState.Deleted)
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());
        return domainEvents;
    }


}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}