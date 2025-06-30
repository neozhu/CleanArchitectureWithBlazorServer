# GitHub Copilot Instructions

> **Purpose:** Ensure Copilot’s suggestions follow this repository’s architecture, folder layout, and coding standards.

---

## 1  Project Snapshot

| Setting        | Value                                                                                |
| -------------- | ------------------------------------------------------------------------------------ |
| **Solution**   | `CleanArchitectureWithBlazorServer.sln`                                              |
| **Runtime**    | .NET 9 · Blazor Server                                                               |
| **UI Library** | MudBlazor (latest stable)                                                            |
| **Patterns**   | Clean Architecture · CQRS · MediatR                                                  |
| **Tooling**    | AutoMapper · FluentValidation · EF Core · Identity.EntityFrameworkCore · FusionCache |

Copilot **must** respect these pillars when proposing code.

---

## 2  Folder / Namespace Map

| Layer / Purpose                                 | Physical Path               | Typical Contents                                                                  | Root Namespace                                           |
| ----------------------------------------------- | --------------------------- | --------------------------------------------------------------------------------- | -------------------------------------------------------- |
| **Domain** (Enterprise core)                    | `src/Domain/`               | Entities, Value Objects, Enums, Domain Events                                     | `CleanArchitectureWithBlazorServer.Domain`               |
| **Application** (Use‑cases)                     | `src/Application/`          | CQRS Commands & Queries, Handlers, DTOs, Validators, Mapping Profiles, Interfaces | `CleanArchitectureWithBlazorServer.Application`          |
| **Infrastructure** (Framework & external calls) | `src/Infrastructure/`       | EF Core persistence, Identity, Mail/File adapters, Background Jobs                | `CleanArchitectureWithBlazorServer.Infrastructure`       |
| **Migrators** (DB migrations)                   | `src/Migrators/<Provider>/` | FluentMigrator projects for MSSQL, PostgreSQL, SQLite                             | `CleanArchitectureWithBlazorServer.Migrators.<Provider>` |
| **UI** (Blazor Server)                          | `src/Server.UI/`            | `.razor` pages, components, `Program.cs`, DI wiring                               | `CleanArchitectureWithBlazorServer.Server.UI`            |
| **Tests**                                       | `tests/`                    | Unit / Integration projects                                                       | Same as tested assembly                                  |

> 🔖 **Rule:** Copilot should place files so the **implicit namespace** equals the **folder’s root namespace**.

---

## 3  Naming Rules

| Artifact         | Suffix / Pattern                               |
| ---------------- | ---------------------------------------------- |
| Command          | `*Command` implements `IRequest<TResult>`      |
| Query            | `*Query` implements `IRequest<TResult>`        |
| Handler          | `*Handler`                                     |
| DTO              | `*Dto`                                         |
| Mapping Profile  | `*Profile`                                     |
| Validator        | `*Validator` (inherits `AbstractValidator<T>`) |
| Blazor Component | `PascalCase.razor`                             |
| Interface        | Prefix `I`                                     |
| DI Extension     | Static class `DependencyInjection`             |

---

## 4  Generation Checklist

When Copilot emits code it **must**:

1. Use **constructor injection**; mark dependencies `readonly`.
2. Stay **async/await** – *never* block with `.Result` or `.Wait()`.
3. Guard arguments using `ArgumentNullException.ThrowIfNull(...)` or inline null‑checks.
4. Provide a **FluentValidation validator** for every Command/Query.
5. Keep **business logic** solely in Domain & Application layers.
6. Register services inside the correct layer’s `DependencyInjection` class.
7. Add **unit tests** (NUnit + Moq or In‑Memory Db) for new handlers/services.
8. Generate **bUnit tests** for Blazor components with meaningful logic.
9. Avoid hard‑coded secrets; use `IConfiguration` / user secrets.
10. Use **FusionCache** (`IFusionCache`) for caching read‑heavy queries; prefer `GetOrSetAsync` with sensible expirations.

---

## 5  CQRS Skeleton Example

```csharp
// src/Application/TodoItems/Commands/CreateTodoItemCommand.cs
public sealed record CreateTodoItemCommand(string Title) : ICacheInvalidatorRequest<Result<int>>;

// src/Application/TodoItems/Commands/CreateTodoItemHandler.cs
internal sealed class CreateTodoItemHandler : IRequestHandler<CreateTodoItemCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateTodoItemHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateTodoItemCommand request, CancellationToken ct)
    {
        var entity = new TodoItem(request.Title);
        _context.TodoItems.Add(entity);
        await _context.SaveChangesAsync(ct);
        return await Result<int>.SuccessAsync(item.Id);
    }
}

// Validator
public sealed class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
```

---

## 6  Blazor + MudBlazor Guidelines

* Inject MediatR in pages/components: `@inject ISender Mediator`.
* Use `MudForm`, `MudTextField`, etc., instead of raw HTML.
* Limit a component to **≤ 300 LOC**; split otherwise.
* Theme via `MudTheme` variables; **no inline colors**.
* Dialogs use `MudDialog` & `DialogParameters`.
* Event handlers are **`async Task`** methods.

---

## 7  FusionCache Guidelines

* Register `FusionCache` (e.g., `services.AddFusionCache()`) in **Infrastructure.DependencyInjection**.
* Prefer injecting `IFusionCache` instead of `IMemoryCache`.
* Use `GetOrSetAsync` for *read‑through* caching; default absolute expiration ≤ 5 minutes unless data is static.
* Build cache keys as `<Layer>:<Entity>:<Id>` or `<Feature>:<Hash>` to avoid collisions.
* Never cache sensitive or per‑user data without proper key scoping (`user:{UserId}:...`).

## 8  EF Core & Identity Guidelines

* **DbContext location:** `src/Infrastructure/Persistence`.
* Configure EF Core in `Infrastructure.DependencyInjection` with `services.AddDbContext<ApplicationDbContext>(...)`.
* Set default tracking: `UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)`; call `.AsTracking()` only when updates are needed.
* **Lazy loading is disabled**; include related data explicitly via `.Include/ThenInclude`.
* Expose only abstractions (`IApplicationDbContext`) to Application layer; never inject `ApplicationDbContext` directly outside Infrastructure.
* Create migrations **inside Infrastructure project** and keep generated scripts under provider‑specific folders in `src/Migrators`.
* **Identity:** add `services.AddIdentityCore<ApplicationUser>()` and `AddEntityFrameworkStores<ApplicationDbContext>()`.

  * `ApplicationUser` inherits `IdentityUser<Guid>`.
  * Seed default roles/admin user in an `IHostedService` (Infrastructure).
  * Policies live in `Server.UI`, constants in `Application`.
* Wrap transactional work in `IDbContextTransaction` or MediatR pipeline behaviors (`UnitOfWorkBehavior`).

## 9  AutoMapper Rules

* Profiles live in `src/Application/Common/Mapping`.
* Call `CreateMap<Source, Destination>().ReverseMap();` when bidirectional.
* Validate mapping config in tests with `configuration.AssertConfigurationIsValid();`.

---

## 10  Testing Matrix

| Target                      | Framework | Helper libs                 |
| --------------------------- | --------- | --------------------------- |
| Handlers & Services         | NUnit     | Moq · FluentAssertions      |
| Blazor Components           | bUnit     | Shouldly/FluentAssertions   |
| Integration (EF, Pipelines) | NUnit     | Testcontainers / InMemoryDb |

---

## 11  Migrators Conventions

* Each provider project (`Migrators.MSSQL`, etc.) holds **only migrations** plus provider‑specific DI.
* Migrations are chronological (`yyyymmddHHmm_description`).
* C# migration classes inherit `Migration` from **FluentMigrator**.
* Each migration **must be idempotent** and reversible (`Down()` implementation).

---

## 12  Commit Message Format

Follow **Conventional Commits**:

```
<type>(scope): <subject>
```

Types: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `chore`, `perf`, `ci`.

---

## 13  What Copilot **MUST NOT** Do

* Mix layers (e.g., make DbContext calls from the UI).
* Propose blocking/synchronous IO.
* Generate monolithic `.razor` files > 300 LOC.
* Hard‑code secrets, connection strings, or magic numbers.

---

When uncertain, Copilot should insert an inline

```csharp
// TODO: Clarify requirement with maintainers.
```

so humans can refine the intent.
