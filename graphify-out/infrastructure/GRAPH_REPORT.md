# Graph Report - src/Infrastructure  (2026-05-29)

## Corpus Check
- Corpus is ~15,075 words - fits in a single context window. You may not need a graph.

## Summary
- 689 nodes · 999 edges · 53 communities (48 shown, 5 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS · INFERRED: 1 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Audit Interceptors|Audit Interceptors]]
- [[_COMMUNITY_Permission Queries|Permission Queries]]
- [[_COMMUNITY_Data Source Services|Data Source Services]]
- [[_COMMUNITY_Client Context|Client Context]]
- [[_COMMUNITY_Permission Service|Permission Service]]
- [[_COMMUNITY_User Profile State|User Profile State]]
- [[_COMMUNITY_Identity Data Sources|Identity Data Sources]]
- [[_COMMUNITY_Project Dependencies|Project Dependencies]]
- [[_COMMUNITY_Serilog Extensions|Serilog Extensions]]
- [[_COMMUNITY_Document OCR AI|Document OCR AI]]
- [[_COMMUNITY_OCR Background Queue|OCR Background Queue]]
- [[_COMMUNITY_File Upload Services|File Upload Services]]
- [[_COMMUNITY_Excel Service|Excel Service]]
- [[_COMMUNITY_Identity Service|Identity Service]]
- [[_COMMUNITY_Permission Assignment|Permission Assignment]]
- [[_COMMUNITY_Dependency Injection|Dependency Injection]]
- [[_COMMUNITY_Application DbContext|Application DbContext]]
- [[_COMMUNITY_Ticket Store Cache|Ticket Store Cache]]
- [[_COMMUNITY_PDF Service|PDF Service]]
- [[_COMMUNITY_Domain Event Dispatch|Domain Event Dispatch]]
- [[_COMMUNITY_Database Initialization|Database Initialization]]
- [[_COMMUNITY_Validation Service|Validation Service]]
- [[_COMMUNITY_Identity Entity Config|Identity Entity Config]]
- [[_COMMUNITY_Audit Sign In|Audit Sign In]]
- [[_COMMUNITY_Fusion Cache|Fusion Cache]]
- [[_COMMUNITY_Picklist Data Source|Picklist Data Source]]
- [[_COMMUNITY_User Context Accessor|User Context Accessor]]
- [[_COMMUNITY_User Context Loader|User Context Loader]]
- [[_COMMUNITY_Mail Service|Mail Service]]
- [[_COMMUNITY_Database Settings|Database Settings]]
- [[_COMMUNITY_User Context Contract|User Context Contract]]
- [[_COMMUNITY_HTTP Context Extensions|HTTP Context Extensions]]
- [[_COMMUNITY_Global Query Filters|Global Query Filters]]
- [[_COMMUNITY_DbContext Factory|DbContext Factory]]
- [[_COMMUNITY_Host Initialization|Host Initialization]]
- [[_COMMUNITY_Audit Trail Config|Audit Trail Config]]
- [[_COMMUNITY_Contact Config|Contact Config]]
- [[_COMMUNITY_Data Protection Config|Data Protection Config]]
- [[_COMMUNITY_System Log Config|System Log Config]]
- [[_COMMUNITY_Tenant User Config|Tenant User Config]]
- [[_COMMUNITY_Document Config|Document Config]]
- [[_COMMUNITY_Picklist Config|Picklist Config]]
- [[_COMMUNITY_Product Config|Product Config]]
- [[_COMMUNITY_Tenant Config|Tenant Config]]
- [[_COMMUNITY_Value Conversions|Value Conversions]]
- [[_COMMUNITY_App Settings|App Settings]]
- [[_COMMUNITY_Identity Settings|Identity Settings]]
- [[_COMMUNITY_User Claim Config|User Claim Config]]
- [[_COMMUNITY_Minio Options|Minio Options]]
- [[_COMMUNITY_User Login Config|User Login Config]]
- [[_COMMUNITY_User Config|User Config]]
- [[_COMMUNITY_SMTP Options|SMTP Options]]

## God Nodes (most connected - your core abstractions)
1. `UserProfileState` - 22 edges
2. `AuditableEntityInterceptor` - 21 edges
3. `DataSourceServiceBase` - 17 edges
4. `IdentityService` - 16 edges
5. `DocumentOcrJob` - 15 edges
6. `DependencyInjection` - 14 edges
7. `ApplicationDbContext` - 14 edges
8. `PermissionAssignmentService` - 13 edges
9. `ApplicationDbContextInitializer` - 12 edges
10. `MemoryCacheTicketStore` - 12 edges

## Surprising Connections (you probably didn't know these)
- `AISettings` --implements--> `IAISettings`  [EXTRACTED]
  Configurations/AISettings.cs → Services/OpenAI/DocumentOcrJob.cs
- `DateTimeService` --implements--> `IDateTime`  [EXTRACTED]
  Services/DateTimeService.cs → Persistence/Interceptors/AuditableEntityInterceptor.cs
- `UserProfileState` --implements--> `IUserProfileState`  [EXTRACTED]
  Services/Identity/UserProfileState.cs → Services/TenantSwitchService.cs
- `PermissionQueryService` --implements--> `IPermissionQueryService`  [EXTRACTED]
  Services/Identity/PermissionQueryService.cs → Services/Identity/PermissionAssignmentService.cs
- `RoleDataSourceService` --inherits--> `DataSourceServiceBase`  [EXTRACTED]
  Services/Identity/RoleDataSourceService.cs → Services/DataSourceServiceBase.cs

## Communities (53 total, 5 thin omitted)

### Community 0 - "Audit Interceptors"
Cohesion: 0.10
Nodes (21): DateTime, DbContextErrorEventData, EntityEntry, HashSet, IAuditableEntity, AuditableEntityInterceptor, Extensions, AuditTrail (+13 more)

### Community 1 - "Permission Queries"
Cohesion: 0.15
Nodes (17): Claim, Field, FieldInfo, ClaimComparer, PermissionQueryService, IEqualityComparer, ModuleInfo, ApplicationRole (+9 more)

### Community 2 - "Data Source Services"
Cohesion: 0.12
Nodes (19): IDataSourceService, TenantDataSourceService, CancellationToken, Expression, Func, FusionCacheEntryOptions, IEnumerable, IFusionCache (+11 more)

### Community 3 - "Client Context"
Cohesion: 0.10
Nodes (18): ClientInfo, Exception, HubInvocationContext, HubLifetimeContext, ClientInfoAccessor, Node, Pop, UserContextHubFilter (+10 more)

### Community 4 - "Permission Service"
Cohesion: 0.10
Nodes (18): AuthenticationStateProvider, IAuthorizationService, ITenantSwitchService, List, Task, PermissionService, ApplicationUser, IApplicationDbContextFactory (+10 more)

### Community 5 - "User Profile State"
Cohesion: 0.16
Nodes (13): UserProfileState, SemaphoreSlim, ApplicationUserDto, CancellationToken, Func, IFusionCache, ILogger, int (+5 more)

### Community 6 - "Identity Data Sources"
Cohesion: 0.09
Nodes (17): ApplicationRoleDto, RoleDataSourceService, UserDataSourceService, IDisposable, CancellationToken, IServiceScopeFactory, List, string (+9 more)

### Community 7 - "Project Dependencies"
Cohesion: 0.09
Nodes (21): net10.0, Blazor.Serilog.Sinks.SQLite (1.0.9), ClosedXML (0.105.0), MailKit (4.16.0), MaxMind.GeoIP2 (5.4.1), Microsoft.Agents.AI.OpenAI (1.5.0), Microsoft.AspNetCore.Authentication.Google (10.0.8), Microsoft.AspNetCore.Authentication.MicrosoftAccount (10.0.8) (+13 more)

### Community 8 - "Serilog Extensions"
Cohesion: 0.17
Nodes (11): IConfiguration, IHttpContextAccessor, SerilogExtensions, UserInfoEnricher, UtcTimestampEnricher, ILogEventEnricher, ILogEventPropertyFactory, LogEvent (+3 more)

### Community 9 - "Document OCR AI"
Cohesion: 0.16
Nodes (14): AISettings, string, IApplicationHubWrapper, IDocumentOcrJob, DocumentOcrJob, CancellationToken, Document, IAISettings (+6 more)

### Community 10 - "OCR Background Queue"
Cohesion: 0.12
Nodes (15): BackgroundService, Channel, DocumentOcrRequest, DocumentOcrBackgroundService, EmptyScope, DocumentOcrQueue, CancellationToken, IDisposable (+7 more)

### Community 11 - "File Upload Services"
Cohesion: 0.11
Nodes (14): IFileUploadService, IMinioClient, BrowserFileUploadService, Result, string, Task, UploadedFileInfo, UploadRequest (+6 more)

### Community 12 - "Excel Service"
Cohesion: 0.16
Nodes (13): DataRow, IExcelService, IResult, IXLCell, Dictionary, Func, IEnumerable, Task (+5 more)

### Community 13 - "Identity Service"
Cohesion: 0.16
Nodes (10): IdentityService, IIdentityService, ApplicationUserDto, CancellationToken, IDictionary, IFusionCache, IServiceScopeFactory, List (+2 more)

### Community 14 - "Permission Assignment"
Cohesion: 0.21
Nodes (10): PermissionAssignmentService, IdentityResult, IReadOnlyCollection, IEnumerable, IList, ILogger, IPermissionQueryService, IServiceScopeFactory (+2 more)

### Community 15 - "Dependency Injection"
Cohesion: 0.27
Nodes (6): DbContextOptionsBuilder, IConfiguration, int, string, DependencyInjection, IServiceCollection

### Community 16 - "Application DbContext"
Cohesion: 0.11
Nodes (14): IApplicationDbContext, IDataProtectionKeyContext, IdentityDbContext, ModelConfigurationBuilder, ApplicationDbContext, ApplicationRole, ApplicationRoleClaim, ApplicationUser (+6 more)

### Community 17 - "Ticket Store Cache"
Cohesion: 0.20
Nodes (10): AuthenticationTicket, FusionCacheOptions, ITicketStore, Lazy, CleanArchitecture.Blazor.Infrastructure.Services, Action, FusionCacheEntryOptions, string (+2 more)

### Community 18 - "PDF Service"
Cohesion: 0.14
Nodes (11): float, IContainer, IPDFService, Dictionary, Func, IEnumerable, int, string (+3 more)

### Community 19 - "Domain Event Dispatch"
Cohesion: 0.18
Nodes (11): DomainEvent, DispatchDomainEventsInterceptor, CancellationToken, DbContext, DbContextEventData, IMediator, InterceptionResult, SaveChangesCompletedEventData (+3 more)

### Community 20 - "Database Initialization"
Cohesion: 0.23
Nodes (7): ApplicationDbContext, ApplicationDbContextInitializer, IEnumerable, ILogger, RoleManager, Task, UserManager

### Community 21 - "Validation Service"
Cohesion: 0.21
Nodes (11): IServiceProvider, IValidationService, Action, CancellationToken, Func, IDictionary, IEnumerable, Task (+3 more)

### Community 22 - "Identity Entity Config"
Cohesion: 0.22
Nodes (9): ApplicationRoleClaimConfiguration, ApplicationRoleConfiguration, ApplicationUserRoleConfiguration, ApplicationUserTokenConfiguration, ApplicationRole, ApplicationRoleClaim, ApplicationUserRole, ApplicationUserToken (+1 more)

### Community 23 - "Audit Sign In"
Cohesion: 0.22
Nodes (9): AuthenticationProperties, AuditSignInManager, IApplicationDbContextFactory, IHttpContextAccessor, ILogger, Task, SignInManager, SignInResult (+1 more)

### Community 24 - "Fusion Cache"
Cohesion: 0.23
Nodes (8): FusionAppCache, CancellationToken, Func, IEnumerable, IFusionCache, T, Task, IAppCache

### Community 25 - "Picklist Data Source"
Cohesion: 0.21
Nodes (10): PicklistSetDto, CancellationToken, Expression, Func, IApplicationDbContextFactory, IEnumerable, List, Task (+2 more)

### Community 26 - "User Context Accessor"
Cohesion: 0.26
Nodes (7): Node, Pop, UserContextAccessor, IUserContextAccessor, AsyncLocal, IDisposable, UserContext

### Community 27 - "User Context Loader"
Cohesion: 0.18
Nodes (8): UserContextLoader, IUserContextLoader, CancellationToken, ClaimsPrincipal, IFusionCache, IServiceScopeFactory, Task, UserContext

### Community 28 - "Mail Service"
Cohesion: 0.24
Nodes (6): IMailService, ILogger, string, Task, MailService, SmtpClientOptions

### Community 29 - "Database Settings"
Cohesion: 0.25
Nodes (6): IEnumerable, string, DatabaseSettings, IValidatableObject, ValidationContext, ValidationResult

### Community 30 - "User Context Contract"
Cohesion: 0.25
Nodes (5): IUserContextLoader, CancellationToken, ClaimsPrincipal, Task, UserContext

### Community 32 - "Global Query Filters"
Cohesion: 0.29
Nodes (5): ModelBuilderExtensions, Expression, Func, ModelBuilder, TInterface

### Community 33 - "DbContext Factory"
Cohesion: 0.29
Nodes (5): IApplicationDbContextFactory, ApplicationDbContextFactory, CancellationToken, IApplicationDbContext, ValueTask

### Community 34 - "Host Initialization"
Cohesion: 0.40
Nodes (3): Task, HostExtensions, IHost

### Community 35 - "Audit Trail Config"
Cohesion: 0.50
Nodes (3): AuditTrailConfiguration, AuditTrail, EntityTypeBuilder

### Community 36 - "Contact Config"
Cohesion: 0.50
Nodes (3): ContactConfiguration, Contact, EntityTypeBuilder

### Community 37 - "Data Protection Config"
Cohesion: 0.50
Nodes (3): DataProtectionKeyConfiguration, DataProtectionKey, EntityTypeBuilder

### Community 38 - "System Log Config"
Cohesion: 0.50
Nodes (3): SystemLogConfiguration, EntityTypeBuilder, SystemLog

### Community 39 - "Tenant User Config"
Cohesion: 0.50
Nodes (3): TenantUserConfiguration, EntityTypeBuilder, TenantUser

### Community 40 - "Document Config"
Cohesion: 0.50
Nodes (3): DocumentConfiguration, Document, EntityTypeBuilder

### Community 41 - "Picklist Config"
Cohesion: 0.50
Nodes (3): PicklistSetConfiguration, EntityTypeBuilder, PicklistSet

### Community 42 - "Product Config"
Cohesion: 0.50
Nodes (3): ProductConfiguration, EntityTypeBuilder, Product

### Community 43 - "Tenant Config"
Cohesion: 0.50
Nodes (3): TenantConfiguration, EntityTypeBuilder, Tenant

### Community 44 - "Value Conversions"
Cohesion: 0.40
Nodes (3): ValueConversionExtensions, T, PropertyBuilder

### Community 45 - "App Settings"
Cohesion: 0.50
Nodes (3): AppConfigurationSettings, string, IApplicationSettings

### Community 46 - "Identity Settings"
Cohesion: 0.50
Nodes (3): string, IdentitySettings, IIdentitySettings

### Community 47 - "User Claim Config"
Cohesion: 0.67
Nodes (3): ApplicationUserClaimConfiguration, IEntityTypeConfiguration, ApplicationUserClaim

## Knowledge Gaps
- **215 isolated node(s):** `string`, `int`, `net10.0`, `Microsoft.Agents.AI.OpenAI (1.5.0)`, `OpenAI (2.10.0)` (+210 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `UserProfileState` connect `User Profile State` to `Permission Service`, `Identity Data Sources`?**
  _High betweenness centrality (0.022) - this node is a cross-community bridge._
- **Why does `DataSourceServiceBase` connect `Data Source Services` to `Picklist Data Source`, `Identity Data Sources`?**
  _High betweenness centrality (0.019) - this node is a cross-community bridge._
- **What connects `string`, `int`, `net10.0` to the rest of the system?**
  _215 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Audit Interceptors` be split into smaller, more focused modules?**
  _Cohesion score 0.09672830725462304 - nodes in this community are weakly interconnected._
- **Should `Data Source Services` be split into smaller, more focused modules?**
  _Cohesion score 0.11904761904761904 - nodes in this community are weakly interconnected._
- **Should `Client Context` be split into smaller, more focused modules?**
  _Cohesion score 0.10256410256410256 - nodes in this community are weakly interconnected._
- **Should `Permission Service` be split into smaller, more focused modules?**
  _Cohesion score 0.09686609686609686 - nodes in this community are weakly interconnected._