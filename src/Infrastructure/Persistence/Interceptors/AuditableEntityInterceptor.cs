using System.Globalization;
using System.Text.Json;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using Mediator;
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
    private readonly IDateTime _dateTime;
    private readonly IMediator _mediator;
    private List<AuditTrail> _temporaryAuditTrailList = new();
    private static readonly HashSet<string> _auditableMetadataFields = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(IAuditableEntity.CreatedAt),
        nameof(IAuditableEntity.CreatedById),
        nameof(IAuditableEntity.LastModifiedAt),
        nameof(IAuditableEntity.LastModifiedById)
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableEntityInterceptor"/> class.
    /// </summary>
    /// <param name="userContextAccessor">The current user context accessor (scoped).</param>
    /// <param name="dateTime">The date and time service.</param>
    public AuditableEntityInterceptor(IUserContextAccessor userContextAccessor, IDateTime dateTime, IMediator mediator)
    {
        _userContextAccessor = userContextAccessor;
        _dateTime = dateTime;
        _mediator = mediator;
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
        _temporaryAuditTrailList.Clear();
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
        entity.CreatedById = userId;
        entity.CreatedAt = now;
        if (entity is IMustHaveTenant mustTenant && mustTenant.TenantId==null) mustTenant.TenantId = tenantId;
        if (entity is IMayHaveTenant mayTenant && mayTenant.TenantId==null) mayTenant.TenantId = tenantId;
    }

    private static void SetModificationAuditInfo(IAuditableEntity entity, string userId, DateTime now)
    {
        entity.LastModifiedById = userId;
        entity.LastModifiedAt = now;
    }

    private static void SetDeletionAuditInfo(EntityEntry entry, string userId, DateTime now)
    {
        if (entry.Entity is ISoftDelete softDelete)
        {
            softDelete.DeletedById = userId;
            softDelete.DeletedAt = now;
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
            Changes = new Dictionary<string, AuditChange>()
        };

        // Set a default audit type based on entry state; property loop can refine details.
        auditTrail.AuditType = entry.State switch
        {
            EntityState.Added => AuditType.Create,
            EntityState.Deleted => AuditType.Delete,
            EntityState.Modified => AuditType.Update,
            _ => auditTrail.AuditType
        };

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary)
            {
                auditTrail.TemporaryProperties.Add(property);
                continue;
            }

            var propertyName = property.Metadata.Name;
            if (entry.State == EntityState.Modified && _auditableMetadataFields.Contains(propertyName))
            {
                continue;
            }
            if (property.Metadata.IsPrimaryKey() && property.CurrentValue != null)
            {
                auditTrail.PrimaryKey[propertyName] = SerializeValue(property.CurrentValue);
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    if (property.CurrentValue != null)
                        auditTrail.Changes[propertyName] = new AuditChange { New = SerializeValue(property.CurrentValue) };
                    break;

                case EntityState.Deleted:
                    if (property.OriginalValue != null)
                        auditTrail.Changes[propertyName] = new AuditChange { Old = SerializeValue(property.OriginalValue) };
                    break;

                case EntityState.Modified when property.IsModified && !Equals(property.OriginalValue, property.CurrentValue):
                    auditTrail.AffectedColumns.Add(propertyName);
                    auditTrail.Changes[propertyName] = new AuditChange
                    {
                        Old = SerializeValue(property.OriginalValue),
                        New = SerializeValue(property.CurrentValue)
                    };
                    break;
            }
        }

        return auditTrail;
    }

    private async Task FinalizeAuditTrailsAsync(DbContext context, CancellationToken cancellationToken)
    {
        if (_temporaryAuditTrailList.Any())
        {
            var resolved = ResolveAuditTrails(_temporaryAuditTrailList);
            var toPublish = resolved.Where(HasChanges).ToList();
            if (toPublish.Any())
            {
                await _mediator.Publish(new AuditTrailsReadyEvent(toPublish), cancellationToken);
            }
            _temporaryAuditTrailList.Clear();
        }
    }


    private static string? SerializeValue(object value)
    {
        if (value is null) return null;

        var type = value.GetType();
        if (type.IsPrimitive || value is string || value is decimal || value is DateTime || value is DateTimeOffset || value is Guid || value is TimeSpan || value is Enum)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        try
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions.Web);
        }
        catch (NotSupportedException)
        {
            // Fallback to readable string when JSON serialization cannot handle the object.
            return value.ToString();
        }
    }

    private static List<AuditTrail> ResolveAuditTrails(IEnumerable<AuditTrail> auditTrails)
    {
        foreach (var auditTrail in auditTrails)
        {
            foreach (var prop in auditTrail.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue != null)
                {
                    auditTrail.PrimaryKey[prop.Metadata.Name] = SerializeValue(prop.CurrentValue);
                }
                else if (prop.CurrentValue != null)
                {
                    auditTrail.Changes ??= new Dictionary<string, AuditChange>();
                    auditTrail.Changes[prop.Metadata.Name] = new AuditChange { New = SerializeValue(prop.CurrentValue) };
                }
            }
        }

        // project to new instances without PropertyEntry references so they are safe for async dispatch
        return auditTrails.Select(a => new AuditTrail
        {
            TableName = a.TableName,
            UserId = a.UserId,
            DateTime = a.DateTime,
            AffectedColumns = a.AffectedColumns?.ToList(),
            Changes = a.Changes?.ToDictionary(kv => kv.Key, kv => new AuditChange { Old = kv.Value.Old, New = kv.Value.New }),
            PrimaryKey = new Dictionary<string, string>(a.PrimaryKey),
            AuditType = a.AuditType
        }).ToList();
    }

    private static bool HasChanges(AuditTrail auditTrail)
    {
        return auditTrail.Changes != null && auditTrail.Changes.Any();
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
