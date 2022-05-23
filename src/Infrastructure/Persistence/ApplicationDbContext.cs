// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

#nullable disable
public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, string,
    ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
    ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext
{

    private readonly IDomainEventService _domainEventService;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventService domainEventService,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
        ) : base(options)
    {
        _domainEventService = domainEventService;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Logger> Loggers { get; set; }
    public DbSet<AuditTrail> AuditTrails { get; set; }
    public DbSet<Document> Documents { get; set; }

    public DbSet<KeyValue> KeyValues { get; set; }

    public DbSet<Product> Products { get; set; }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
   
        //var auditEntries = OnBeforeSaveChanges(userName);
       
        var events = ChangeTracker.Entries<IHasDomainEvent>()
               .Select(x => x.Entity.DomainEvents)
               .SelectMany(x => x)
               .Where(domainEvent => !domainEvent.IsPublished)
               .ToArray();
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchEvents(events);
        //await OnAfterSaveChanges(auditEntries, cancellationToken);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);
       
    }

    private List<AuditTrail> OnBeforeSaveChanges(string userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditTrail>();
        foreach (var entry in ChangeTracker.Entries<IAuditTrial>())
        {
            if (entry.Entity is AuditTrail ||
                entry.State == EntityState.Detached ||
                entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditTrail()
            {
                //DateTime = _dateTime.Now,
                TableName = entry.Entity.GetType().Name,
                UserId = userId,
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

        foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
        {
            AuditTrails.Add(auditEntry);
        }
        return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }
    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var _event in events)
        {
            _event.IsPublished = true;
            await _domainEventService.Publish(_event);
        }
    }
    private Task OnAfterSaveChanges(List<AuditTrail> auditEntries, CancellationToken cancellationToken = new())
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

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
            AuditTrails.Add(auditEntry);
        }
        return SaveChangesAsync(cancellationToken);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

}
