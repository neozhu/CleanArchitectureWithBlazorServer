# Graph Report - src/Domain  (2026-05-29)

## Corpus Check
- Corpus is ~2,582 words - fits in a single context window. You may not need a graph.

## Summary
- 114 nodes · 98 edges · 24 communities (15 shown, 9 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Domain Events|Domain Events]]
- [[_COMMUNITY_Project Dependencies|Project Dependencies]]
- [[_COMMUNITY_Auditable Entities|Auditable Entities]]
- [[_COMMUNITY_Audit And Tenancy|Audit And Tenancy]]
- [[_COMMUNITY_Base Entity Events|Base Entity Events]]
- [[_COMMUNITY_Product Files|Product Files]]
- [[_COMMUNITY_Soft Delete Entities|Soft Delete Entities]]
- [[_COMMUNITY_Mediator Notifications|Mediator Notifications]]
- [[_COMMUNITY_Role Claims|Role Claims]]
- [[_COMMUNITY_Application Users|Application Users]]
- [[_COMMUNITY_User Logins|User Logins]]
- [[_COMMUNITY_Application Roles|Application Roles]]
- [[_COMMUNITY_User Claims|User Claims]]
- [[_COMMUNITY_User Roles|User Roles]]
- [[_COMMUNITY_User Tokens|User Tokens]]

## God Nodes (most connected - your core abstractions)
1. `BaseAuditableEntity` - 8 edges
2. `IEntity` - 8 edges
3. `BaseEntity` - 7 edges
4. `Document` - 4 edges
5. `BaseAuditableSoftDeleteEntity` - 3 edges
6. `IAuditTrial` - 3 edges
7. `PicklistSet` - 3 edges
8. `DomainEvent` - 2 edges
9. `UploadedFileInfo` - 2 edges
10. `IAuditableEntity` - 2 edges

## Surprising Connections (you probably didn't know these)
- `Product` --inherits--> `BaseAuditableEntity`  [EXTRACTED]
  Entities/Product.cs → Common/Entities/BaseAuditableEntity.cs
- `Contact` --inherits--> `BaseAuditableEntity`  [EXTRACTED]
  Entities/Contact.cs → Common/Entities/BaseAuditableEntity.cs
- `AuditTrail` --implements--> `IEntity`  [EXTRACTED]
  Entities/AuditTrail.cs → Common/Entities/IEntity.cs
- `SystemLog` --implements--> `IEntity`  [EXTRACTED]
  Entities/SystemLog.cs → Common/Entities/IEntity.cs
- `Tenant` --implements--> `IEntity`  [EXTRACTED]
  Entities/Tenant.cs → Common/Entities/IEntity.cs

## Communities (24 total, 9 thin omitted)

### Community 0 - "Domain Events"
Cohesion: 0.09
Nodes (12): DomainEvent, ContactCreatedEvent, ContactDeletedEvent, ContactUpdatedEvent, DocumentCreatedEvent, DocumentDeletedEvent, PicklistSetCreatedEvent, PicklistSetDeletedEvent (+4 more)

### Community 1 - "Project Dependencies"
Cohesion: 0.12
Nodes (16): net10.0, EFCore.NamingConventions (10.0.1), EntityFrameworkCore.Exceptions.PostgreSQL (10.0.0), EntityFrameworkCore.Exceptions.Sqlite (10.0.0), EntityFrameworkCore.Exceptions.SqlServer (10.0.0), Mediator.Abstractions (3.0.2), Microsoft.AspNetCore.DataProtection.EntityFrameworkCore (10.0.8), Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore (10.0.8) (+8 more)

### Community 2 - "Auditable Entities"
Cohesion: 0.16
Nodes (8): BaseAuditableEntity, IAuditableEntity, Contact, Document, IAuditTrial, IMayHaveTenant, IMustHaveTenant, PicklistSet

### Community 3 - "Audit And Tenancy"
Cohesion: 0.20
Nodes (6): AuditChange, AuditTrail, IEntity, SystemLog, Tenant, TenantUser

### Community 4 - "Base Entity Events"
Cohesion: 0.33
Nodes (3): DomainEvent, BaseEntity, List

### Community 5 - "Product Files"
Cohesion: 0.40
Nodes (3): UploadedFileInfo, Product, ProductImage

## Knowledge Gaps
- **19 isolated node(s):** `net10.0`, `Mediator.Abstractions (3.0.2)`, `Microsoft.EntityFrameworkCore (10.0.8)`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore (10.0.8)`, `EntityFrameworkCore.Exceptions.PostgreSQL (10.0.0)` (+14 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **9 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `BaseAuditableEntity` connect `Auditable Entities` to `Base Entity Events`, `Product Files`, `Soft Delete Entities`?**
  _High betweenness centrality (0.091) - this node is a cross-community bridge._
- **Why does `BaseEntity` connect `Base Entity Events` to `Auditable Entities`, `Audit And Tenancy`?**
  _High betweenness centrality (0.074) - this node is a cross-community bridge._
- **Why does `IEntity` connect `Audit And Tenancy` to `Base Entity Events`?**
  _High betweenness centrality (0.054) - this node is a cross-community bridge._
- **What connects `net10.0`, `Mediator.Abstractions (3.0.2)`, `Microsoft.EntityFrameworkCore (10.0.8)` to the rest of the system?**
  _19 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Domain Events` be split into smaller, more focused modules?**
  _Cohesion score 0.08695652173913043 - nodes in this community are weakly interconnected._
- **Should `Project Dependencies` be split into smaller, more focused modules?**
  _Cohesion score 0.11764705882352941 - nodes in this community are weakly interconnected._