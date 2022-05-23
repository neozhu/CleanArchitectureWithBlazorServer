using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private List<AuditTrail>? _auditTrailList;
    public AuditableEntitySaveChangesInterceptor(
        ITenantProvider tenantProvider,
        ICurrentUserService currentUserService,
        IDateTime dateTime)
    {
        _tenantProvider = tenantProvider;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var userId = await _currentUserService.UserId();
        var tenantId = await _tenantProvider.GetTenant();
        UpdateEntities(eventData.Context, userId, tenantId);
        _auditTrailList = InsertAuditTrail(eventData.Context, userId);
       return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await UpdateTemporaryPropertiesForAuditTrail(eventData.Context, _auditTrailList, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
    private void UpdateEntities(DbContext? context,string userId,string tenantId)
    {
        if (context == null) return;
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
            }
        }
    }

    private List<AuditTrail> InsertAuditTrail(DbContext? context,string userId)
    {
        if (context == null) return new List<AuditTrail>();
        context.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditTrail>();
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
                AffectedColumns = new List<string>()
            };
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {

                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.PrimaryKey[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            auditEntry.AffectedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        foreach (var auditEntry in auditEntries.Where(x=>!x.HasTemporaryProperties))
        {
           context.Add(auditEntry);
        }
        return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }

    private async Task UpdateTemporaryPropertiesForAuditTrail(DbContext? context,List<AuditTrail>? auditEntries, CancellationToken cancellationToken = default)
    {
        if (context is null || auditEntries == null || auditEntries.Count == 0) return;
          

        foreach (var auditEntry in auditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    auditEntry.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
            context.Add(auditEntry);
            await context.SaveChangesAsync(cancellationToken);
        }
       
    }
}
