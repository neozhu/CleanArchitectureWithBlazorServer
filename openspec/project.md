# Project Context

## Purpose
Clean Architecture template for Blazor Server targeting production-ready line-of-business apps. It provides opinionated building blocks for authentication/authorization, multi-tenancy, data access, caching, background jobs, reporting, observability, and UI scaffolding so teams can focus on domain features rather than plumbing.

## Tech Stack
- .NET 9, C# (LangVersion: default; Nullable: enable)
- Blazor Server (Razor Components with interactive server render mode)
- EF Core 9 with providers: SQLite, SQL Server, PostgreSQL; EFCore.NamingConventions
- Identity: ASP.NET Core Identity (custom AuditSignInManager, multi-tenant claims factory)
- CQRS/Messaging: MediatR with pre-processors and pipeline behaviors
- Validation: FluentValidation
- Mapping/Utilities: AutoMapper, Ardalis.Specification, Scriban
- Caching: ZiggyCreatures.FusionCache (request caching + invalidation behaviors)
- Background jobs: Hangfire (InMemory by default)
- Logging/Observability: Serilog (Console, Async, MSSqlServer, PostgreSQL, SQLite, Seq)
- Realtime: SignalR
- UI: MudBlazor, ApexCharts; Localization via Resources
- Files/Docs/Media: MinIO (S3-compatible) for uploads, QuestPDF, SixLabors.ImageSharp
- Mail: MailKit/MimeKit
- Security/AI: Security analysis heuristics; Gemini API client used for OCR workloads
- CI/Security: GitHub Actions (build on main), CodeQL weekly analysis

## Project Conventions

### Code Style
- C#: Nullable enabled; implicit usings enabled; LangVersion default (per SDK).
- Naming: PascalCase for types/namespaces; interfaces prefixed with I; options classes end with Options/Settings; DTOs suffixed with Dto.
- DI pattern: Each layer exposes a static `DependencyInjection` class with `AddX`/`ConfigureX` extension methods on `IServiceCollection`/`WebApplication`.
- Configuration binding: `IOptions<T>` with strongly-typed settings (e.g., `IdentitySettings`, `DatabaseSettings`, `AISettings`, `MinioOptions`). Appsettings section names must match constants used in DI.
- Resources/Localization: `Resources` folder with `.resx` or JSON resources; `ResourcesPath` configured via `LocalizationConstants`.
- File headers: Some files include MIT license header; keep consistent if modifying those files.

### Architecture Patterns
- Clean Architecture layering:
  - Domain: core types, identity models, domain primitives.
  - Application: CQRS with MediatR, validators, pipeline behaviors (Validation, Performance, FusionCache, CacheInvalidation), exception handlers, specs, DTOs.
  - Infrastructure: EF Core DbContext, persistence/interceptors, identity, external services (MinIO, Mail, PDF, OCR, Geolocation), configuration.
  - Server.UI: Blazor Server app (Razor Components), SignalR hubs, middleware, Hangfire dashboard, UI services.
  - Migrators: Separate projects for provider-specific migrations (SQLite, SQL Server, PostgreSQL).
- Specification pattern via `Ardalis.Specification` for query composition.
- Caching integrated at MediatR pipeline level using FusionCache; explicit invalidation behavior included.
- Exception handling centralized with `IExceptionHandler` implementations and ProblemDetails.
- Security: ASP.NET Core Identity with custom `AuditSignInManager`, lockout and password policies via settings, login auditing and risk analysis.

### Testing Strategy
- Test projects under `tests/` by layer:
  - `Application.UnitTests`, `Application.IntegrationTests` (NUnit, Moq, FluentAssertions, Respawn for DB reset).
  - `Domain.UnitTests` (NUnit, FluentAssertions).
  - `Infrastructure.UnitTests` (xUnit, Moq).
- Aim for unit coverage on pipeline behaviors, exception handlers, validators, and heuristics. Use integration tests for EF-backed queries/commands and DI wiring.
- Run locally with `dotnet test` at the solution level. CI currently builds; tests can be added to workflow if needed.

### Git Workflow
- Default branch: `main`. Build workflow runs on push/PR to `main` with .NET 9 SDK.
- CodeQL security analysis runs on push/PR to `main` and weekly on a schedule.
- Use feature branches and PRs for changes; keep changes scoped and ensure build passes. No enforced commit message convention specified.

## Domain Context
- Identity and Access Management: Users, Roles, Permissions; email confirmation required. Custom sign-in manager records login audits.
- Multi-tenancy: User claims principal factory enriches identity for tenant context; tenant switching and data source services are provided.
- Auditing and Security Risk: Login audits analyzed by heuristics to detect brute-force attempts, unusual times, new devices/locations; thresholds configured in `SecurityAnalysis` settings. Dashboard components surface risk levels and summaries.
- Files and Documents: Uploads stored via MinIO; PDF generation with QuestPDF; Image processing with ImageSharp; OCR jobs integrate Gemini HTTP endpoint.
- Notifications and Email: MailKit SMTP with templated emails (embedded Razor templates) for user activation, recovery, MFA codes, welcome.
- Realtime and UX: SignalR hub for notifications and activity; MudBlazor for UI components; ApexCharts for analytics.

## Important Constraints
- Target framework is `net9.0` across projects; use SDK 9.x locally and in CI.
- Supported DB providers: `sqlite`, `sqlserver`, `npgsql`; selectable via `DatabaseSettings.DBProvider` and migrations assemblies.
- Default Hangfire storage is in-memory; configure persistent storage for production.
- SignalR max receive message size set to 64 KB; adjust if sending larger payloads.
- Identity policies (password, lockout) and risk thresholds are configuration-driven; changes may impact security posture.
- External services (MinIO, Seq, Gemini API) require valid credentials/URLs via configuration.

## External Dependencies
- Databases: SQLite, SQL Server, PostgreSQL (via EF Core). Connection and provider from `DatabaseSettings` in `appsettings.json`.
- Object Storage: MinIO (`Minio` section: `Endpoint`, `AccessKey`, `SecretKey`, `BucketName`).
- Logging: Serilog sinks (Console/Async/SQLite/PostgreSQL/MSSqlServer/Seq); Seq server URL configured under `Serilog:WriteTo`.
- Background Jobs: Hangfire (ASP.NET Core integration; InMemory by default; dashboard at `/jobs`, authorization filters provided).
- Realtime: ASP.NET Core SignalR for server hub communications.
- Email: SMTP via MailKit (`SmtpClientOptions` section); templated emails embedded in `Server.UI/Resources/EmailTemplates`.
- AI/OCR: Google Generative Language API (Gemini 2.5 Flash endpoint) with API key in `AI:GeminiApiKey`.
- Geolocation: HTTP client with custom user-agent for geolocation lookups used by security analysis.

## New Entity/Feature Guide (Contacts Pattern)

Use this step-by-step guide to add a new domain entity, application feature set, and Blazor Server pages following the established Contacts implementation. Replace `Contact(s)` with your entity name, keep namespaces and folders aligned, and mirror the same patterns for caching, specs, security, and UI.

1. Domain
- 1.1 Entity (`src/Domain/Entities/<Entity>.cs`)
  - Derive from `BaseAuditableEntity`.
  - Keep domain clean: no data annotations; use EF configuration for constraints.
  - Example reference: `src/Domain/Entities/Contact.cs:8`.
- 1.2 Domain events (`src/Domain/Events/<Entity>CreatedEvent.cs|UpdatedEvent.cs|DeletedEvent.cs`)
  - Derive each from `DomainEvent` and expose a read-only `<Entity> Item`.
  - Raise in command handlers on create/update/delete operations.
  - Example references: `src/Domain/Events/ContactCreatedEvent.cs:18`, `src/Domain/Events/ContactUpdatedEvent.cs:18`, `src/Domain/Events/ContactDeletedEvent.cs:18`.

2. Infrastructure
- 2.1 EF configuration (`src/Infrastructure/Persistence/Configurations/<Entity>Configuration.cs`)
  - Implement `IEntityTypeConfiguration<<Entity>>`.
  - Configure required fields, lengths, indexes; ignore `DomainEvents`.
  - Example: `src/Infrastructure/Persistence/Configurations/ContactConfiguration.cs:9`.
- 2.2 Seed data (`src/Infrastructure/Persistence/ApplicationDbContextInitializer.cs`)
  - Add an initialization block in `SeedDataAsync()` to create representative test rows if none exist.
  - Use `_context.<Entities>.AddRangeAsync(...)` then `_context.SaveChangesAsync()`; guard with `if (!await _context.<Entities>.AnyAsync())`.
  - Example insertion patterns: see product seeding in `src/Infrastructure/Persistence/ApplicationDbContextInitializer.cs:277`.

3. Application
- 3.1 Caching (`src/Application/Features/<Entities>/Caching/<Entity>CacheKey.cs`)
  - Provide `GetAllCacheKey`, per-query keys, `Tags` array, and `Refresh()` delegating to `FusionCacheFactory.RemoveByTags(Tags)`.
  - Example: `src/Application/Features/Contacts/Caching/ContactCacheKey.cs:1`.
- 3.2 Commands (`src/Application/Features/<Entities>/Commands/...`)
  - Add `AddEdit/<AddEditEntityCommand>.cs`: implements `ICacheInvalidatorRequest<Result<int>>`; maps DTO→entity; raises `Created/Updated` events; uses `<Entity>CacheKey` and `Tags`.
  - Optionally add separate `Create/`, `Update/`, and `Delete/` commands for clearer semantics; `Delete` accepts ids array and raises `<Entity>DeletedEvent` per item.
  - Provide `Import/` commands using `IExcelService` and an optional `Create<Entity>sTemplateCommand`.
  - Examples: `src/Application/Features/Contacts/Commands/AddEdit/AddEditContactCommand.cs:1`, `.../Commands/Delete/DeleteContactCommand.cs`, `.../Commands/Import/ImportContactsCommand.cs`.
  - Database access: Inject and use `IApplicationDbContextFactory` in handlers; do not inject `ApplicationDbContext` directly. Always create per-operation contexts and dispose with `await using`:
    - `private readonly IApplicationDbContextFactory _dbContextFactory;`
    - `await using var db = await _dbContextFactory.CreateAsync(cancellationToken);`
    - Rationale: ensures proper lifetime, tenant scoping, and avoids concurrency issues in server-side Blazor.
- 3.3 DTOs (`src/Application/Features/<Entities>/DTOs/<Entity>Dto.cs`)
  - Define properties for UI/data transfer; add AutoMapper profile mapping `Entity↔Dto`.
  - Ignore audit fields and `DomainEvents` when mapping to entity.
  - Example: `src/Application/Features/Contacts/DTOs/ContactDto.cs:1`.
- 3.4 Event handlers (`src/Application/Features/<Entities>/EventHandlers/`)
  - Create `...Created/Updated/DeletedEventHandler.cs` implementing `INotificationHandler<T>`; log minimal info or trigger side effects.
  - Examples: `src/Application/Features/Contacts/EventHandlers/ContactCreatedEventHandler.cs` et al.
- 3.5 Queries (`src/Application/Features/<Entities>/Queries/...`)
  - `GetById/`: `ICacheableRequest<Result<Dto>>` with per-id cache key; use specification + `ProjectTo<Dto>`.
  - `GetAll/`: return list of DTOs; cache with `GetAllCacheKey`.
  - `Pagination/`: define `PaginationQuery` consuming an advanced filter; return `PaginatedData<Dto>`.
  - `Export/`: produce Excel via `IExcelService` and Scriban/ClosedXML as needed.
  - Examples: `src/Application/Features/Contacts/Queries/GetById/GetContactByIdQuery.cs:1`, `.../Queries/GetAll/GetAllContactsQuery.cs`, `.../Queries/Pagination/ContactsPaginationQuery.cs`, `.../Queries/Export/ExportContactsQuery.cs`.
  - Database access: Follow the same factory pattern as commands. Example usage: `await using var db = await _dbContextFactory.CreateAsync(cancellationToken);` in `GetContactByIdQueryHandler` and pagination handlers.
- 3.6 Security (`src/Application/Features/<Entities>/Security/<Entities>Permissions.cs`)
  - Add a nested static class to `Permissions` with actions: `View, Create, Clone, Edit, Delete, Print, Search, Export, Import`.
  - Include a matching `<Entities>AccessRights` POCO used by UI to toggle capabilities.
  - Example: `src/Application/Features/Contacts/Security/ContactsPermissions.cs:1`.
- 3.7 Specifications (`src/Application/Features/<Entities>/Specifications/...`)
  - Add `<Entity>ByIdSpecification` and an advanced spec that composes keyword, ownership, and date-range filters.
  - Add an advanced filter model and optional list-view enum to drive UI.
  - Examples: `src/Application/Features/Contacts/Specifications/ContactByIdSpecification.cs:1`, `.../ContactAdvancedSpecification.cs:1`, `.../ContactAdvancedFilter.cs:1`.

4. Server.UI (Blazor Server)
- 4.1 Index/list page (`src/Server.UI/Pages/<Entities>/<Entities>.razor`)
  - Use `MudDataGrid<TDto>` with server-side data (`ServerData`), toolbar actions respecting `<Entities>AccessRights`, and `Mediator` for queries/commands.
  - Authorize with `[Authorize(Policy = Permissions.<Entities>.View)]`.
  - Example: `src/Server.UI/Pages/Contacts/Contacts.razor:1`.
- 4.2 Create page (`src/Server.UI/Pages/<Entities>/Create<Entity>.razor`)
  - Show `ContactFormDialog`-like component or page form; post `AddEdit<Entity>Command` via `Mediator`.
  - Authorize with `Permissions.<Entities>.Create`.
  - Example: `src/Server.UI/Pages/Contacts/CreateContact.razor:1`.
- 4.3 Edit page (`src/Server.UI/Pages/<Entities>/Edit<Entity>.razor`)
  - Load DTO by id using `Get<Entity>ByIdQuery`; bind to form and send `AddEdit<Entity>Command`.
  - Authorize with `Permissions.<Entities>.Edit`.
  - Example: `src/Server.UI/Pages/Contacts/EditContact.razor:1`.
- 4.4 Readonly view (`src/Server.UI/Pages/<Entities>/View<Entity>.razor`)
  - Consume `Get<Entity>ByIdQuery`; render fields using read-only components (e.g., `ReadOnlyField`).
  - Provide Edit/Delete actions gated by access rights.
  - Example: `src/Server.UI/Pages/Contacts/ViewContact.razor:1`.
- 4.5 Dialog component (`src/Server.UI/Pages/<Entities>/Components/<Entity>FormDialog.razor`)
  - Encapsulate the edit form; bind to `AddEdit<Entity>Command` model; validate via `IValidationService` and `MudForm`.
  - Return success via `MudDialog.Close(DialogResult.Ok(true))`; surface errors via `Snackbar`.
  - Example: `src/Server.UI/Pages/Contacts/Components/ContactFormDialog.razor:1`.

Additional notes
- Menus: Add navigation entry under `src/Server.UI/Services/Navigation/MenuService.cs:38` for discoverability.
- Permissions seeding: New permissions are auto-picked from the `Permissions` nested classes; no extra wiring required beyond definitions.
- Caching: Use `<Entity>CacheKey.Tags` consistently across cacheable/invalidator requests; call `Refresh()` after bulk ops if needed.
- Migrations: Add/Update migrations in the provider-specific Migrators projects and bump snapshots.
