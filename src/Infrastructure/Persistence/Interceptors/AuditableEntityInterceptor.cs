using CleanArchitecture.Blazor.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
#nullable disable warnings

/// <summary>
/// Interceptor for auditing entity changes.
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private List<AuditTrail> _temporaryAuditTrailList = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableEntityInterceptor"/> class.
    /// </summary>
    /// <param name="currentUserService">The current user service.</param>
    /// <param name="dateTime">The date and time service.</param>
    public AuditableEntityInterceptor(ICurrentUserService currentUserService, IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    /// <inheritdoc/>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        UpdateAuditableEntities(context);
        _temporaryAuditTrailList = GenerateAuditTrails(context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var saveResult = await base.SavedChangesAsync(eventData, result, cancellationToken);
        if (context != null)
        {
            await FinalizeAuditTrailsAsync(context, cancellationToken);
        }
        return saveResult;
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        var userId = _currentUserService.UserId;
        var tenantId = _currentUserService.TenantId;
        var now = _dateTime.Now;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreationAuditInfo(entry.Entity, userId, tenantId, now);
                    break;

                case EntityState.Modified:
                    SetModificationAuditInfo(entry.Entity, userId, now);
                    break;

                case EntityState.Deleted:
                    SetDeletionAuditInfo(entry, userId, now);
                    break;

                case EntityState.Unchanged:
                    if (entry.HasChangedOwnedEntities())
                    {
                        SetModificationAuditInfo(entry.Entity, userId, now);
                    }
                    break;
            }
        }
    }

    private static void SetCreationAuditInfo(BaseAuditableEntity entity, string userId, string tenantId, DateTime now)
    {
        entity.CreatedBy = userId;
        entity.Created = now;
        if (entity is IMustHaveTenant mustTenant) mustTenant.TenantId = tenantId;
        if (entity is IMayHaveTenant mayTenant) mayTenant.TenantId = tenantId;
    }

    private static void SetModificationAuditInfo(BaseAuditableEntity entity, string userId, DateTime now)
    {
        entity.LastModifiedBy = userId;
        entity.LastModified = now;
    }

    private static void SetDeletionAuditInfo(EntityEntry entry, string userId, DateTime now)
    {
        if (entry.Entity is ISoftDelete softDelete)
        {
            softDelete.DeletedBy = userId;
            softDelete.Deleted = now;
            entry.State = EntityState.Modified;
        }
    }

    private List<AuditTrail> GenerateAuditTrails(DbContext context)
    {
        var userId = _currentUserService.UserId;
        var now = _dateTime.Now;
        var auditTrails = new List<AuditTrail>();

        foreach (var entry in context.ChangeTracker.Entries<IAuditTrial>())
        {
            if (IsValidAuditEntry(entry))
            {
                var auditTrail = CreateAuditTrail(entry, userId, now);
                auditTrails.Add(auditTrail);
            }
        }

        return auditTrails;
    }

    private static bool IsValidAuditEntry(EntityEntry entry)
    {
        return entry.Entity is not AuditTrail && entry.State != EntityState.Detached && entry.State != EntityState.Unchanged;
    }

    private AuditTrail CreateAuditTrail(EntityEntry entry, string userId, DateTime now)
    {
        var auditTrail = new AuditTrail
        {
            TableName = entry.Entity.GetType().Name,
            UserId = userId,
            DateTime = now,
            AffectedColumns = new List<string>(),
            NewValues = new Dictionary<string, object?>(),
            OldValues = new Dictionary<string, object?>()
        };

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary)
            {
                auditTrail.TemporaryProperties.Add(property);
                continue;
            }

            var propertyName = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey() && property.CurrentValue != null)
            {
                auditTrail.PrimaryKey[propertyName] = property.CurrentValue;
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditTrail.AuditType = AuditType.Create;
                    if (property.CurrentValue != null) auditTrail.NewValues[propertyName] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    auditTrail.AuditType = AuditType.Delete;
                    if (property.OriginalValue != null) auditTrail.OldValues[propertyName] = property.OriginalValue;
                    break;

                case EntityState.Modified when property.IsModified && !Equals(property.OriginalValue, property.CurrentValue):
                    auditTrail.AuditType = AuditType.Update;
                    auditTrail.AffectedColumns.Add(propertyName);
                    if (property.OriginalValue != null) auditTrail.OldValues[propertyName] = property.OriginalValue;
                    if (property.CurrentValue != null) auditTrail.NewValues[propertyName] = property.CurrentValue;
                    break;
            }
        }

        return auditTrail;
    }

    private async Task FinalizeAuditTrailsAsync(DbContext context, CancellationToken cancellationToken)
    {
        if (_temporaryAuditTrailList.Any())
        {
            foreach (var auditTrail in _temporaryAuditTrailList)
            {
                foreach (var prop in auditTrail.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue != null)
                    {
                        auditTrail.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (auditTrail.NewValues != null && prop.CurrentValue != null)
                    {
                        auditTrail.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
            }

            await context.AddRangeAsync(_temporaryAuditTrailList, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            _temporaryAuditTrailList.Clear();
        }
    }
}

public static class Extensions
{
    /// <summary>
    /// Checks if the entity entry has any owned entities that have been added or modified.
    /// </summary>
    /// <param name="entry">The entity entry.</param>
    /// <returns><c>true</c> if the entity entry has changed owned entities; otherwise, <c>false</c>.</returns>
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}
