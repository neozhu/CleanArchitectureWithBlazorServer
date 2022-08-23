using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
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
    private List<AuditTrail>? _temporaryAuditTrailList;
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
        var userId = await _currentUserService.UserId();
        var tenantId = await _tenantProvider.GetTenantId();
        if (eventData.Context is not null)
        {
            UpdateEntities(eventData.Context, userId, tenantId);
            _temporaryAuditTrailList = InsertTemporaryAuditTrail(eventData.Context, userId, tenantId);
        }
       return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null && _temporaryAuditTrailList is not null)
        {
            await UpdateTemporaryPropertiesForAuditTrail(eventData.Context, _temporaryAuditTrailList, cancellationToken);
        }
        var resultvalueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        if (eventData.Context is not null)
        {
            await _mediator.DispatchDomainEvents(eventData.Context);
        }
        return resultvalueTask;
    }
    private void UpdateEntities(DbContext context,string userId,string tenantId)
    {
        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created = _dateTime.Now;
                    if (entry.Entity is IMustHaveTenant mustenant)
                    {
                        mustenant.TenantId = tenantId;
                    }
                    if (entry.Entity is IMayHaveTenant maytenant && !string.IsNullOrEmpty(tenantId))
                    {
                        maytenant.TenantId = tenantId;
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

    private List<AuditTrail> InsertTemporaryAuditTrail(DbContext context,string userId, string tenantId)
    {
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
            temporaryAuditEntries.Add(auditEntry);
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

                    case EntityState.Modified when property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false:
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
        }
       
        foreach (var auditEntry in temporaryAuditEntries.Where(x=>!x.HasTemporaryProperties))
        {
           context.Add(auditEntry);
        }
        return temporaryAuditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }

    private async Task UpdateTemporaryPropertiesForAuditTrail(DbContext context,List<AuditTrail> auditEntries, CancellationToken cancellationToken = default)
    {
        foreach (var auditEntry in auditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue is not null)
                {
                    auditEntry.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                }
                else if(auditEntry.NewValues is not null &&  prop.CurrentValue is not null)
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
            context.Add(auditEntry);
            await context.SaveChangesAsync(cancellationToken);
        }
       
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
