# Graph Report - src/Application/Features  (2026-05-29)

## Corpus Check
- Corpus is ~13,090 words - fits in a single context window. You may not need a graph.

## Summary
- 685 nodes · 889 edges · 72 communities (55 shown, 17 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Add Edit Validators|Add Edit Validators]]
- [[_COMMUNITY_Command Handlers|Command Handlers]]
- [[_COMMUNITY_Audit Export Query|Audit Export Query]]
- [[_COMMUNITY_Specifications|Specifications]]
- [[_COMMUNITY_Contact Export Query|Contact Export Query]]
- [[_COMMUNITY_Product Queries|Product Queries]]
- [[_COMMUNITY_Product Tenant Paging|Product Tenant Paging]]
- [[_COMMUNITY_Product And Log Queries|Product And Log Queries]]
- [[_COMMUNITY_Document Pagination|Document Pagination]]
- [[_COMMUNITY_Audit Pagination|Audit Pagination]]
- [[_COMMUNITY_Contact Import|Contact Import]]
- [[_COMMUNITY_Picklist Pagination|Picklist Pagination]]
- [[_COMMUNITY_System Log Pagination|System Log Pagination]]
- [[_COMMUNITY_Product Import|Product Import]]
- [[_COMMUNITY_Picklist Import|Picklist Import]]
- [[_COMMUNITY_Picklist Events|Picklist Events]]
- [[_COMMUNITY_Document Add Edit|Document Add Edit]]
- [[_COMMUNITY_Tenant Add Edit|Tenant Add Edit]]
- [[_COMMUNITY_Contact List Query|Contact List Query]]
- [[_COMMUNITY_Picklist List Query|Picklist List Query]]
- [[_COMMUNITY_Contact Detail Query|Contact Detail Query]]
- [[_COMMUNITY_Reset Password Notification|Reset Password Notification]]
- [[_COMMUNITY_Picklist Name Query|Picklist Name Query]]
- [[_COMMUNITY_Two Factor Notification|Two Factor Notification]]
- [[_COMMUNITY_Welcome Notification|Welcome Notification]]
- [[_COMMUNITY_Tenant List Query|Tenant List Query]]
- [[_COMMUNITY_User Activation Notification|User Activation Notification]]
- [[_COMMUNITY_User DTO Mapping|User DTO Mapping]]
- [[_COMMUNITY_Picklist Add Edit|Picklist Add Edit]]
- [[_COMMUNITY_Contact Create|Contact Create]]
- [[_COMMUNITY_Document Upload|Document Upload]]
- [[_COMMUNITY_Document Created Event|Document Created Event]]
- [[_COMMUNITY_Product Add Edit|Product Add Edit]]
- [[_COMMUNITY_Contact Update|Contact Update]]
- [[_COMMUNITY_Contact Delete|Contact Delete]]
- [[_COMMUNITY_Document Delete|Document Delete]]
- [[_COMMUNITY_Audit Ready Event|Audit Ready Event]]
- [[_COMMUNITY_Contact Created Event|Contact Created Event]]
- [[_COMMUNITY_Product Created Event|Product Created Event]]
- [[_COMMUNITY_Contact Cache Keys|Contact Cache Keys]]
- [[_COMMUNITY_Contact Deleted Event|Contact Deleted Event]]
- [[_COMMUNITY_Contact Updated Event|Contact Updated Event]]
- [[_COMMUNITY_Document Deleted Event|Document Deleted Event]]
- [[_COMMUNITY_Product Deleted Event|Product Deleted Event]]
- [[_COMMUNITY_Product Updated Event|Product Updated Event]]
- [[_COMMUNITY_Tenant DTO Equality|Tenant DTO Equality]]
- [[_COMMUNITY_Audit Permissions|Audit Permissions]]
- [[_COMMUNITY_Document Cache Keys|Document Cache Keys]]
- [[_COMMUNITY_Log Cache Keys|Log Cache Keys]]
- [[_COMMUNITY_Product Cache Keys|Product Cache Keys]]
- [[_COMMUNITY_Contact Permissions|Contact Permissions]]
- [[_COMMUNITY_Document Permissions|Document Permissions]]
- [[_COMMUNITY_Product Permissions|Product Permissions]]
- [[_COMMUNITY_Log Permissions|Log Permissions]]
- [[_COMMUNITY_Picklist Permissions|Picklist Permissions]]
- [[_COMMUNITY_Tenant Permissions|Tenant Permissions]]
- [[_COMMUNITY_Audit Cache Keys|Audit Cache Keys]]
- [[_COMMUNITY_Picklist Cache Keys|Picklist Cache Keys]]
- [[_COMMUNITY_Tenant Cache Keys|Tenant Cache Keys]]
- [[_COMMUNITY_Audit DTO|Audit DTO]]
- [[_COMMUNITY_Role DTO|Role DTO]]
- [[_COMMUNITY_Contact DTO|Contact DTO]]
- [[_COMMUNITY_Document DTO|Document DTO]]
- [[_COMMUNITY_User Brief DTO|User Brief DTO]]
- [[_COMMUNITY_Picklist DTO|Picklist DTO]]
- [[_COMMUNITY_Product DTO|Product DTO]]
- [[_COMMUNITY_System Log DTO|System Log DTO]]
- [[_COMMUNITY_Timeline DTO|Timeline DTO]]

## God Nodes (most connected - your core abstractions)
1. `ImportContactsCommandHandler` - 11 edges
2. `ExportContactsQueryHandler` - 10 edges
3. `ImportProductsCommandHandler` - 10 edges
4. `ExportProductsQueryHandler` - 10 edges
5. `ImportPicklistSetsCommandHandler` - 9 edges
6. `PicklistSetChangedEventHandler` - 9 edges
7. `GetAllProductsQueryHandler` - 9 edges
8. `ExportAuditTrailsQueryHandler` - 8 edges
9. `AuditTrailsWithPaginationQuery` - 8 edges
10. `AuditTrailsQueryHandler` - 8 edges

## Surprising Connections (you probably didn't know these)
- `AuditTrailsWithPaginationQuery` --inherits--> `AuditTrailAdvancedFilter`  [EXTRACTED]
  AuditTrails/Queries/PaginationQuery/AuditTrailsWithPaginationQuery.cs → AuditTrails/Specifications/AuditTrailAdvancedFilter.cs
- `ExportContactsQuery` --inherits--> `ContactAdvancedFilter`  [EXTRACTED]
  Contacts/Queries/Export/ExportContactsQuery.cs → Contacts/Specifications/ContactAdvancedFilter.cs
- `ContactsWithPaginationQuery` --inherits--> `ContactAdvancedFilter`  [EXTRACTED]
  Contacts/Queries/Pagination/ContactsPaginationQuery.cs → Contacts/Specifications/ContactAdvancedFilter.cs
- `DocumentsWithPaginationQuery` --inherits--> `AdvancedDocumentsFilter`  [EXTRACTED]
  Documents/Queries/PaginationQuery/DocumentsWithPaginationQuery.cs → Documents/Specifications/AdvancedDocumentsFilter.cs
- `PicklistSetsWithPaginationQuery` --inherits--> `PicklistSetAdvancedFilter`  [EXTRACTED]
  PicklistSets/Queries/PaginationQuery/PicklistSetsWithPaginationQuery.cs → PicklistSets/Specifications/PicklistSetAdvancedFilter.cs

## Communities (72 total, 17 thin omitted)

### Community 0 - "Add Edit Validators"
Cohesion: 0.04
Nodes (33): AbstractValidator, AddEditContactCommandValidator, AddEditDocumentCommandValidator, AddEditPicklistSetCommandValidator, AddEditProductCommandValidator, AddEditTenantCommandValidator, AddEditContactCommand, AddEditDocumentCommand (+25 more)

### Community 1 - "Command Handlers"
Cohesion: 0.09
Nodes (26): AddEditContactCommand, AddEditContactCommandHandler, ClearSystemLogsCommand, ClearSystemLogsCommandHandler, CancellationToken, IApplicationDbContextFactory, IObjectMapper, Result (+18 more)

### Community 2 - "Audit Export Query"
Cohesion: 0.08
Nodes (25): CancellationToken, IApplicationDbContextFactory, IExcelService, IStringLocalizer, TypeAdapterConfig, ValueTask, ExportAuditTrailsQuery, ExportAuditTrailsQueryHandler (+17 more)

### Community 3 - "Specifications"
Cohesion: 0.07
Nodes (22): AuditTrail, Contact, Contact, CancellationToken, Document, IApplicationDbContextFactory, ValueTask, Document (+14 more)

### Community 4 - "Contact Export Query"
Cohesion: 0.11
Nodes (19): CancellationToken, ContactDto, IApplicationDbContextFactory, IExcelService, IStringLocalizer, Result, TypeAdapterConfig, ValueTask (+11 more)

### Community 5 - "Product Queries"
Cohesion: 0.11
Nodes (19): ExportProductsQuery, ExportProductsQueryHandler, IPDFService, ProductsWithPaginationQuery, ProductsWithPaginationQueryHandler, CancellationToken, IApplicationDbContextFactory, IExcelService (+11 more)

### Community 6 - "Product Tenant Paging"
Cohesion: 0.13
Nodes (17): DeleteProductCommand, DeleteProductCommandHandler, IRequestHandler, TenantsPaginationSpecification, TenantsWithPaginationQuery, TenantsWithPaginationQueryHandler, CancellationToken, IApplicationDbContextFactory (+9 more)

### Community 7 - "Product And Log Queries"
Cohesion: 0.17
Nodes (17): SystemLogsChatDataQueryHandler, SystemLogsTimeLineChatDataQuery, GetAllProductsQuery, GetAllProductsQueryHandler, GetProductQuery, ICacheableRequest, List, CancellationToken (+9 more)

### Community 8 - "Document Pagination"
Cohesion: 0.21
Nodes (10): DocumentDto, CancellationToken, IApplicationDbContextFactory, PaginatedData, TypeAdapterConfig, ValueTask, PaginationFilter, DocumentsQueryHandler (+2 more)

### Community 9 - "Audit Pagination"
Cohesion: 0.23
Nodes (9): AuditTrailDto, CancellationToken, IApplicationDbContextFactory, PaginatedData, TypeAdapterConfig, ValueTask, AuditTrailsQueryHandler, AuditTrailsWithPaginationQuery (+1 more)

### Community 10 - "Contact Import"
Cohesion: 0.22
Nodes (11): CancellationToken, ContactDto, IApplicationDbContextFactory, IExcelService, IObjectMapper, IStringLocalizer, Result, ValueTask (+3 more)

### Community 11 - "Picklist Pagination"
Cohesion: 0.23
Nodes (9): PicklistSetsQueryHandler, PicklistSetsWithPaginationQuery, CancellationToken, IApplicationDbContextFactory, PaginatedData, PicklistSetDto, TypeAdapterConfig, ValueTask (+1 more)

### Community 12 - "System Log Pagination"
Cohesion: 0.23
Nodes (9): LogsQueryHandler, SystemLogsWithPaginationQuery, SystemLogAdvancedFilter, SystemLogDto, CancellationToken, IApplicationDbContextFactory, PaginatedData, TypeAdapterConfig (+1 more)

### Community 13 - "Product Import"
Cohesion: 0.24
Nodes (10): CreateProductsTemplateCommand, ImportProductsCommand, ImportProductsCommandHandler, CancellationToken, IApplicationDbContextFactory, IExcelService, IObjectMapper, IStringLocalizer (+2 more)

### Community 14 - "Picklist Import"
Cohesion: 0.25
Nodes (9): ImportPicklistSetsCommand, ImportPicklistSetsCommandHandler, IValidator, CancellationToken, IApplicationDbContextFactory, IExcelService, IStringLocalizer, Result (+1 more)

### Community 15 - "Picklist Events"
Cohesion: 0.27
Nodes (8): PicklistSetChangedEventHandler, PicklistSetCreatedEvent, PicklistSetDeletedEvent, CancellationToken, IDataSourceService, ILogger, ValueTask, PicklistSetUpdatedEvent

### Community 16 - "Document Add Edit"
Cohesion: 0.29
Nodes (8): AddEditDocumentCommand, AddEditDocumentCommandHandler, CancellationToken, IApplicationDbContextFactory, IObjectMapper, IStringLocalizer, Result, ValueTask

### Community 17 - "Tenant Add Edit"
Cohesion: 0.29
Nodes (8): AddEditTenantCommand, AddEditTenantCommandHandler, CancellationToken, IApplicationDbContextFactory, IDataSourceService, IObjectMapper, Result, ValueTask

### Community 18 - "Contact List Query"
Cohesion: 0.33
Nodes (8): CancellationToken, ContactDto, IApplicationDbContextFactory, IEnumerable, TypeAdapterConfig, ValueTask, GetAllContactsQuery, GetAllContactsQueryHandler

### Community 19 - "Picklist List Query"
Cohesion: 0.33
Nodes (8): GetAllPicklistSetsQuery, GetAllPicklistSetsQueryHandler, CancellationToken, IApplicationDbContextFactory, IEnumerable, PicklistSetDto, TypeAdapterConfig, ValueTask

### Community 20 - "Contact Detail Query"
Cohesion: 0.33
Nodes (8): CancellationToken, ContactDto, IApplicationDbContextFactory, Result, TypeAdapterConfig, ValueTask, GetContactByIdQuery, GetContactByIdQueryHandler

### Community 21 - "Reset Password Notification"
Cohesion: 0.22
Nodes (8): CancellationToken, IApplicationSettings, ILogger, IMailService, IStringLocalizer, ValueTask, ResetPasswordNotificationHandler, ResetPasswordNotification

### Community 22 - "Picklist Name Query"
Cohesion: 0.33
Nodes (8): PicklistSetsQueryByName, PicklistSetsQueryByNameHandler, CancellationToken, IApplicationDbContextFactory, IEnumerable, PicklistSetDto, TypeAdapterConfig, ValueTask

### Community 23 - "Two Factor Notification"
Cohesion: 0.22
Nodes (8): CancellationToken, IApplicationSettings, ILogger, IMailService, IStringLocalizer, ValueTask, SendFactorCodeNotificationHandler, SendFactorCodeNotification

### Community 24 - "Welcome Notification"
Cohesion: 0.22
Nodes (8): CancellationToken, IApplicationSettings, ILogger, IMailService, IStringLocalizer, ValueTask, SendWelcomeNotificationHandler, SendWelcomeNotification

### Community 25 - "Tenant List Query"
Cohesion: 0.33
Nodes (8): GetAllTenantsQuery, GetAllTenantsQueryHandler, CancellationToken, IApplicationDbContextFactory, IEnumerable, TenantDto, TypeAdapterConfig, ValueTask

### Community 26 - "User Activation Notification"
Cohesion: 0.22
Nodes (8): CancellationToken, IApplicationSettings, ILogger, IMailService, IStringLocalizer, ValueTask, UserActivationNotificationHandler, UserActivationNotification

### Community 27 - "User DTO Mapping"
Cohesion: 0.20
Nodes (6): ApplicationUserDto, TenantDto, IReadOnlyCollection, UserProfile, ValidationContext, ValidationResult

### Community 28 - "Picklist Add Edit"
Cohesion: 0.33
Nodes (7): AddEditPicklistSetCommand, AddEditPicklistSetCommandHandler, CancellationToken, IApplicationDbContextFactory, IObjectMapper, Result, ValueTask

### Community 29 - "Contact Create"
Cohesion: 0.33
Nodes (7): CancellationToken, IApplicationDbContextFactory, IObjectMapper, Result, ValueTask, CreateContactCommand, CreateContactCommandHandler

### Community 30 - "Document Upload"
Cohesion: 0.33
Nodes (7): CancellationToken, IApplicationDbContextFactory, Result, ValueTask, IFileUploadService, UploadDocumentCommand, UploadDocumentCommandHandler

### Community 31 - "Document Created Event"
Cohesion: 0.25
Nodes (7): DocumentCreatedEvent, CancellationToken, ILogger, ValueTask, DocumentCreatedEventHandler, IDocumentOcrQueue, IUserContextAccessor

### Community 32 - "Product Add Edit"
Cohesion: 0.33
Nodes (7): AddEditProductCommand, AddEditProductCommandHandler, CancellationToken, IApplicationDbContextFactory, IObjectMapper, Result, ValueTask

### Community 33 - "Contact Update"
Cohesion: 0.33
Nodes (7): CancellationToken, IApplicationDbContextFactory, IObjectMapper, Result, ValueTask, UpdateContactCommand, UpdateContactCommandHandler

### Community 34 - "Contact Delete"
Cohesion: 0.39
Nodes (6): CancellationToken, IApplicationDbContextFactory, Result, ValueTask, DeleteContactCommand, DeleteContactCommandHandler

### Community 35 - "Document Delete"
Cohesion: 0.39
Nodes (6): DeleteDocumentCommand, DeleteDocumentCommandHandler, CancellationToken, IApplicationDbContextFactory, Result, ValueTask

### Community 36 - "Audit Ready Event"
Cohesion: 0.29
Nodes (6): CancellationToken, IApplicationDbContextFactory, ILogger, ValueTask, AuditTrailsReadyEvent, AuditTrailsReadyEventHandler

### Community 37 - "Contact Created Event"
Cohesion: 0.29
Nodes (6): ContactCreatedEvent, CancellationToken, ILogger, ValueTask, ContactCreatedEventHandler, INotificationHandler

### Community 38 - "Product Created Event"
Cohesion: 0.29
Nodes (6): ProductCreatedEventHandler, ProductCreatedEvent, CancellationToken, ILogger, ValueTask, Stopwatch

### Community 40 - "Contact Deleted Event"
Cohesion: 0.33
Nodes (5): ContactDeletedEvent, CancellationToken, ILogger, ValueTask, ContactDeletedEventHandler

### Community 41 - "Contact Updated Event"
Cohesion: 0.33
Nodes (5): CancellationToken, ILogger, ValueTask, ContactUpdatedEvent, ContactUpdatedEventHandler

### Community 42 - "Document Deleted Event"
Cohesion: 0.33
Nodes (5): DocumentDeletedEvent, CancellationToken, ILogger, ValueTask, DocumentDeletedEventHandler

### Community 43 - "Product Deleted Event"
Cohesion: 0.33
Nodes (5): ProductDeletedEventHandler, ProductDeletedEvent, CancellationToken, ILogger, ValueTask

### Community 44 - "Product Updated Event"
Cohesion: 0.33
Nodes (5): ProductUpdatedEventHandler, CancellationToken, ILogger, ValueTask, ProductUpdatedEvent

### Community 46 - "Audit Permissions"
Cohesion: 0.40
Nodes (4): string, AuditTrails, AuditTrailsAccessRights, Permissions

### Community 50 - "Contact Permissions"
Cohesion: 0.40
Nodes (4): string, Contacts, ContactsAccessRights, Permissions

### Community 51 - "Document Permissions"
Cohesion: 0.40
Nodes (4): string, Documents, DocumentsAccessRights, Permissions

### Community 52 - "Product Permissions"
Cohesion: 0.40
Nodes (4): string, Permissions, Products, ProductsAccessRights

### Community 53 - "Log Permissions"
Cohesion: 0.40
Nodes (4): Logs, LogsAccessRights, Permissions, string

### Community 54 - "Picklist Permissions"
Cohesion: 0.40
Nodes (4): string, Permissions, PicklistSets, PicklistSetsAccessRights

### Community 55 - "Tenant Permissions"
Cohesion: 0.40
Nodes (4): Permissions, Tenants, TenantsAccessRights, string

## Knowledge Gaps
- **286 isolated node(s):** `string`, `AuditTrailDto`, `IApplicationDbContextFactory`, `ILogger`, `CancellationToken` (+281 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **17 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `GetFileStreamQueryHandler` connect `Specifications` to `Product Tenant Paging`?**
  _High betweenness centrality (0.024) - this node is a cross-community bridge._
- **Why does `TenantsPaginationSpecification` connect `Product Tenant Paging` to `Specifications`?**
  _High betweenness centrality (0.021) - this node is a cross-community bridge._
- **What connects `string`, `AuditTrailDto`, `IApplicationDbContextFactory` to the rest of the system?**
  _286 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Add Edit Validators` be split into smaller, more focused modules?**
  _Cohesion score 0.04081632653061224 - nodes in this community are weakly interconnected._
- **Should `Command Handlers` be split into smaller, more focused modules?**
  _Cohesion score 0.0873440285204991 - nodes in this community are weakly interconnected._
- **Should `Audit Export Query` be split into smaller, more focused modules?**
  _Cohesion score 0.07741935483870968 - nodes in this community are weakly interconnected._
- **Should `Specifications` be split into smaller, more focused modules?**
  _Cohesion score 0.06881720430107527 - nodes in this community are weakly interconnected._