# Graph Report - src/Application/Common  (2026-05-29)

## Corpus Check
- Corpus is ~10,640 words - fits in a single context window. You may not need a graph.

## Summary
- 466 nodes · 551 edges · 68 communities (46 shown, 22 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS · INFERRED: 1 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_No Wait Publisher|No Wait Publisher]]
- [[_COMMUNITY_Database Exception Handling|Database Exception Handling]]
- [[_COMMUNITY_Queryable Extensions|Queryable Extensions]]
- [[_COMMUNITY_Result Model|Result Model]]
- [[_COMMUNITY_Identity Service Contract|Identity Service Contract]]
- [[_COMMUNITY_User Cache Keys|User Cache Keys]]
- [[_COMMUNITY_Validation Extensions|Validation Extensions]]
- [[_COMMUNITY_Excel Service Contract|Excel Service Contract]]
- [[_COMMUNITY_Validation Service Contract|Validation Service Contract]]
- [[_COMMUNITY_Date Extensions|Date Extensions]]
- [[_COMMUNITY_User Profile State|User Profile State]]
- [[_COMMUNITY_Cache Contract|Cache Contract]]
- [[_COMMUNITY_Not Found Handling|Not Found Handling]]
- [[_COMMUNITY_Data Source Contract|Data Source Contract]]
- [[_COMMUNITY_Object Mapping|Object Mapping]]
- [[_COMMUNITY_Fallback Exception Handling|Fallback Exception Handling]]
- [[_COMMUNITY_Validation Exception Handling|Validation Exception Handling]]
- [[_COMMUNITY_Specification Extensions|Specification Extensions]]
- [[_COMMUNITY_Permission Service Contract|Permission Service Contract]]
- [[_COMMUNITY_Result Failure Factory|Result Failure Factory]]
- [[_COMMUNITY_Description Extensions|Description Extensions]]
- [[_COMMUNITY_File Upload Contract|File Upload Contract]]
- [[_COMMUNITY_PDF Service Contract|PDF Service Contract]]
- [[_COMMUNITY_Permissions Registry|Permissions Registry]]
- [[_COMMUNITY_Cache Request Contracts|Cache Request Contracts]]
- [[_COMMUNITY_Client Info Accessor|Client Info Accessor]]
- [[_COMMUNITY_Document OCR Queue|Document OCR Queue]]
- [[_COMMUNITY_Permission Query Contract|Permission Query Contract]]
- [[_COMMUNITY_User Context Accessor|User Context Accessor]]
- [[_COMMUNITY_Application DbContext Contract|Application DbContext Contract]]
- [[_COMMUNITY_DbContext Factory Contract|DbContext Factory Contract]]
- [[_COMMUNITY_Tenant Switch Contract|Tenant Switch Contract]]
- [[_COMMUNITY_Paginated Data|Paginated Data]]
- [[_COMMUNITY_App Strings|App Strings]]
- [[_COMMUNITY_Localization Constants|Localization Constants]]
- [[_COMMUNITY_Data Row Extensions|Data Row Extensions]]
- [[_COMMUNITY_Enum Extensions|Enum Extensions]]
- [[_COMMUNITY_Identity Result Extensions|Identity Result Extensions]]
- [[_COMMUNITY_Application Hub Contract|Application Hub Contract]]
- [[_COMMUNITY_Document OCR Job|Document OCR Job]]
- [[_COMMUNITY_Dashboard Permissions|Dashboard Permissions]]
- [[_COMMUNITY_Role Permissions|Role Permissions]]
- [[_COMMUNITY_User Permissions|User Permissions]]
- [[_COMMUNITY_Exception Handler State|Exception Handler State]]
- [[_COMMUNITY_Mail Service Contract|Mail Service Contract]]
- [[_COMMUNITY_Mapster Configuration|Mapster Configuration]]
- [[_COMMUNITY_Pagination Filter|Pagination Filter]]
- [[_COMMUNITY_Claim Types|Claim Types]]
- [[_COMMUNITY_Database Provider Keys|Database Provider Keys]]
- [[_COMMUNITY_Role Constants|Role Constants]]
- [[_COMMUNITY_User Constants|User Constants]]
- [[_COMMUNITY_Not Found Exception|Not Found Exception]]
- [[_COMMUNITY_Pagination Request|Pagination Request]]
- [[_COMMUNITY_Request Authorization|Request Authorization]]
- [[_COMMUNITY_Global Variables|Global Variables]]
- [[_COMMUNITY_AI Settings Contract|AI Settings Contract]]
- [[_COMMUNITY_Application Settings Contract|Application Settings Contract]]
- [[_COMMUNITY_Date Time Contract|Date Time Contract]]
- [[_COMMUNITY_Identity Settings Contract|Identity Settings Contract]]
- [[_COMMUNITY_Result Contract|Result Contract]]
- [[_COMMUNITY_Organization Item|Organization Item]]
- [[_COMMUNITY_Upload Request|Upload Request]]
- [[_COMMUNITY_Module Info|Module Info]]
- [[_COMMUNITY_Permission Model|Permission Model]]

## God Nodes (most connected - your core abstractions)
1. `Result` - 15 edges
2. `UserCacheKeys` - 11 edges
3. `DbExceptionHandler` - 11 edges
4. `T` - 11 edges
5. `ChannelBasedNoWaitPublisher` - 10 edges
6. `QueryableExtensions` - 9 edges
7. `IIdentityService` - 9 edges
8. `T` - 8 edges
9. `DateTimeExtensions` - 7 edges
10. `IUserProfileState` - 7 edges

## Surprising Connections (you probably didn't know these)
- `MapsterObjectMapper` --implements--> `IObjectMapper`  [EXTRACTED]
  Mappings/MapsterObjectMapper.cs → Interfaces/IObjectMapper.cs

## Communities (68 total, 22 thin omitted)

### Community 0 - "No Wait Publisher"
Cohesion: 0.09
Nodes (17): Channel, ChannelWriter, INotificationPublisher, int, ChannelBasedNoWaitPublisher, CancellationToken, ILogger, NotificationHandlers (+9 more)

### Community 1 - "Database Exception Handling"
Cohesion: 0.13
Nodes (12): DbUpdateException, CancellationToken, RequestExceptionHandlerState, string, TException, TRequest, TResponse, ValueTask (+4 more)

### Community 2 - "Queryable Extensions"
Cohesion: 0.22
Nodes (11): CancellationToken, Func, IQueryable, T, Task, TResult, TypeAdapterConfig, QueryableExtensions (+3 more)

### Community 3 - "Result Model"
Cohesion: 0.31
Nodes (7): IResult, Action, Func, T, Task, TResult, Result

### Community 4 - "Identity Service Contract"
Cohesion: 0.24
Nodes (6): ApplicationUserDto, IIdentityService, CancellationToken, IDictionary, List, Task

### Community 5 - "User Cache Keys"
Cohesion: 0.31
Nodes (3): string, UserCacheKeys, UserCacheType

### Community 6 - "Validation Extensions"
Cohesion: 0.18
Nodes (10): CancellationToken, Dictionary, IEnumerable, List, Task, TRequest, ValidationExtensions, IValidator (+2 more)

### Community 7 - "Excel Service Contract"
Cohesion: 0.23
Nodes (9): DataRow, Dictionary, Func, IEnumerable, IResult, Task, TData, IExcelService (+1 more)

### Community 8 - "Validation Service Contract"
Cohesion: 0.23
Nodes (9): Action, CancellationToken, Func, IDictionary, IEnumerable, Task, TRequest, IValidationService (+1 more)

### Community 9 - "Date Extensions"
Cohesion: 0.39
Nodes (5): DateTime, End, DateTimeExtensions, Start, TimeSpan

### Community 10 - "User Profile State"
Cohesion: 0.27
Nodes (5): IUserProfileState, CancellationToken, Func, Task, UserProfile

### Community 11 - "Cache Contract"
Cohesion: 0.27
Nodes (6): IAppCache, CancellationToken, Func, IEnumerable, T, Task

### Community 12 - "Not Found Handling"
Cohesion: 0.22
Nodes (8): CancellationToken, ILogger, RequestExceptionHandlerState, TException, TRequest, TResponse, ValueTask, NotFoundExceptionHandler

### Community 13 - "Data Source Contract"
Cohesion: 0.22
Nodes (7): CancellationToken, Expression, Func, IEnumerable, T, Task, IDataSourceService

### Community 14 - "Object Mapping"
Cohesion: 0.18
Nodes (7): TDestination, TSource, IObjectMapper, TDestination, TSource, TypeAdapterConfig, MapsterObjectMapper

### Community 15 - "Fallback Exception Handling"
Cohesion: 0.20
Nodes (8): CancellationToken, ILogger, RequestExceptionHandlerState, TException, TRequest, TResponse, ValueTask, FallbackExceptionHandler

### Community 16 - "Validation Exception Handling"
Cohesion: 0.24
Nodes (7): CancellationToken, RequestExceptionHandlerState, TException, TRequest, TResponse, ValueTask, ValidationExceptionHandler

### Community 17 - "Specification Extensions"
Cohesion: 0.25
Nodes (6): Expression, Func, T, SpecificationBuilderExtensions, ISpecificationBuilder, PropertyInfo

### Community 18 - "Permission Service Contract"
Cohesion: 0.31
Nodes (4): List, T, Task, IPermissionService

### Community 19 - "Result Failure Factory"
Cohesion: 0.25
Nodes (5): ConcurrentDictionary, Func, TResponse, ResultFailureFactory, Type

### Community 20 - "Description Extensions"
Cohesion: 0.29
Nodes (5): Expression, Func, T, DescriptionAttributeExtensions, TProperty

### Community 21 - "File Upload Contract"
Cohesion: 0.29
Nodes (5): Result, Task, IFileUploadService, UploadedFileInfo, UploadRequest

### Community 22 - "PDF Service Contract"
Cohesion: 0.25
Nodes (6): Dictionary, Func, IEnumerable, Task, TData, IPDFService

### Community 23 - "Permissions Registry"
Cohesion: 0.32
Nodes (6): List, string, EmailTemplates, Hangfire, NavigationMenu, Permissions

### Community 24 - "Cache Request Contracts"
Cohesion: 0.29
Nodes (5): ICacheableRequest, ICacheInvalidatorRequest, TResponse, TResponse, IRequest

### Community 25 - "Client Info Accessor"
Cohesion: 0.33
Nodes (3): ClientInfo, IClientInfoAccessor, IDisposable

### Community 26 - "Document OCR Queue"
Cohesion: 0.43
Nodes (4): DocumentOcrRequest, CancellationToken, ValueTask, IDocumentOcrQueue

### Community 27 - "Permission Query Contract"
Cohesion: 0.43
Nodes (4): IList, Task, IPermissionQueryService, PermissionModel

### Community 28 - "User Context Accessor"
Cohesion: 0.33
Nodes (3): IUserContextAccessor, IDisposable, UserContext

### Community 29 - "Application DbContext Contract"
Cohesion: 0.33
Nodes (4): IAsyncDisposable, CancellationToken, Task, IApplicationDbContext

### Community 30 - "DbContext Factory Contract"
Cohesion: 0.33
Nodes (4): IApplicationDbContext, CancellationToken, ValueTask, IApplicationDbContextFactory

### Community 31 - "Tenant Switch Contract"
Cohesion: 0.40
Nodes (3): Result, Task, ITenantSwitchService

### Community 32 - "Paginated Data"
Cohesion: 0.33
Nodes (4): IQueryable, T, Task, PaginatedData

### Community 33 - "App Strings"
Cohesion: 0.40
Nodes (3): AppStrings, string, ResourceManager

### Community 34 - "Localization Constants"
Cohesion: 0.40
Nodes (4): string, LanguageCode, LocalizationConstants, LanguageCode

### Community 35 - "Data Row Extensions"
Cohesion: 0.40
Nodes (3): DataRow, T, DataRowExtensions

### Community 36 - "Enum Extensions"
Cohesion: 0.40
Nodes (3): Enum, ConcurrentDictionary, EnumExtensions

### Community 37 - "Identity Result Extensions"
Cohesion: 0.40
Nodes (3): Result, IdentityResultExtensions, IdentityResult

### Community 39 - "Document OCR Job"
Cohesion: 0.40
Nodes (3): CancellationToken, Task, IDocumentOcrJob

### Community 40 - "Dashboard Permissions"
Cohesion: 0.40
Nodes (4): Dashboards, DashboardsAccessRights, Permissions, string

### Community 41 - "Role Permissions"
Cohesion: 0.40
Nodes (4): Permissions, Roles, RolesAccessRights, string

### Community 42 - "User Permissions"
Cohesion: 0.40
Nodes (4): Permissions, Users, UsersAccessRights, string

## Knowledge Gaps
- **152 isolated node(s):** `string`, `string`, `ResourceManager`, `string`, `GlobalVariables` (+147 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **22 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **What connects `string`, `string`, `ResourceManager` to the rest of the system?**
  _152 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `No Wait Publisher` be split into smaller, more focused modules?**
  _Cohesion score 0.09486166007905138 - nodes in this community are weakly interconnected._
- **Should `Database Exception Handling` be split into smaller, more focused modules?**
  _Cohesion score 0.12987012987012986 - nodes in this community are weakly interconnected._