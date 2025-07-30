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
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDateTime _dateTime;
    private List<AuditTrail> _temporaryAuditTrailList = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableEntityInterceptor"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving scoped services.</param>
    /// <param name="dateTime">The date and time service.</param>
    public AuditableEntityInterceptor(IServiceProvider serviceProvider, IDateTime dateTime)
    {
        _userContextAccessor = serviceProvider.GetRequiredService<IUserContextAccessor>();
        _serviceProvider = serviceProvider;
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
    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        await base.SaveChangesFailedAsync(eventData, cancellationToken);
        var context = eventData.Context;
        var exception = eventData.Exception;
        if (context != null)
        {
            var errorMessage = exception.InnerException!=null? exception.InnerException.Message:exception.Message;
            foreach (var auditTrail in _temporaryAuditTrailList)
            {
                auditTrail.ErrorMessage = errorMessage;
            }
            await SaveAuditTrailsWithNewContextAsync(_temporaryAuditTrailList, cancellationToken);
        }

       
    }
    private void UpdateAuditableEntities(DbContext context)
    {
        var currentUser = _userContextAccessor.Current;
        var userId = currentUser?.UserId;
        var tenantId = currentUser?.TenantId;
        var now = _dateTime.Now;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
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

                case EntityState.Unchanged when entry.HasChangedOwnedEntities():
                    SetModificationAuditInfo(entry.Entity, userId, now);
                    break;
            }
        }
    }

    private static void SetCreationAuditInfo(IAuditableEntity entity, string userId, string tenantId, DateTime now)
    {
        entity.CreatedBy = userId;
        entity.Created = now;
        if (entity is IMustHaveTenant mustTenant && mustTenant.TenantId==null) mustTenant.TenantId = tenantId;
        if (entity is IMayHaveTenant mayTenant && mayTenant.TenantId==null) mayTenant.TenantId = tenantId;
    }

    private static void SetModificationAuditInfo(IAuditableEntity entity, string userId, DateTime now)
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
        var currentUser = _userContextAccessor.Current;
        var userId = currentUser?.UserId;
        var now = _dateTime.Now;
        var auditTrails = new List<AuditTrail>();

        foreach (var entry in context.ChangeTracker.Entries<IAuditTrial>())
        {
            if (IsValidAuditEntry(entry))
            {
                var auditTrail = CreateAuditTrail(entry, userId, now,entry.DebugView.LongView);
                auditTrails.Add(auditTrail);
            }
        }

        return auditTrails;
    }

    private static bool IsValidAuditEntry(EntityEntry entry)
    {
        return entry.Entity is not AuditTrail && entry.State != EntityState.Detached && entry.State != EntityState.Unchanged;
    }

    private AuditTrail CreateAuditTrail(EntityEntry entry, string userId, DateTime now,string? debugView)
    {
        var auditTrail = new AuditTrail
        {
            TableName = entry.Entity.GetType().Name,
            UserId = userId,
            DateTime = now,
            AffectedColumns = new List<string>(),
            NewValues = new Dictionary<string, object?>(),
            OldValues = new Dictionary<string, object?>(),
            DebugView = debugView
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
    private async Task SaveAuditTrailsWithNewContextAsync(List<AuditTrail> auditTrails, CancellationToken cancellationToken)
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
            // 使用 IServiceProvider 创建 scope 并获取 IDbContextFactory<ApplicationDbContext>
            using var scope = _serviceProvider.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var dbcontext = dbContextFactory.CreateDbContext();
            await dbcontext.AddRangeAsync(auditTrails, cancellationToken);
            await dbcontext.SaveChangesAsync(cancellationToken);

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
