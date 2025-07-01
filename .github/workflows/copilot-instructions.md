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

8. Avoid hard-coded secrets; use `IConfiguration` / user secrets.

9. Use **FusionCache** (`IFusionCache`) for caching read-heavy queries; prefer `GetOrSetAsync` with sensible expirations.

10. All annotations and comments in the codebase should be written in English to maintain consistency and ensure readability for a global developer audience.

---

## 5  CQRS Skeleton Example

```csharp
// src/Application/Todo/Commands/AddTodoItemCommand.cs
public sealed record AddTodoItemCommand(string Title) : IRequest<int>;

// src/Application/Todo/Commands/AddTodoItemHandler.cs
internal sealed class AddTodoItemHandler : IRequestHandler<AddTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context;
    public AddTodoItemHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(AddTodoItemCommand request, CancellationToken ct)
    {
        var entity = new TodoItem(request.Title);
        _context.TodoItems.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }
}

// src/Application/Todo/Commands/AddTodoItemCommandValidator.cs
public sealed class AddTodoItemCommandValidator : AbstractValidator<AddTodoItemCommand>
{
    public AddTodoItemCommandValidator() =>
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
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

## 7  FusionCache Guidelines

* Register `FusionCache` (e.g., `services.AddFusionCache()`) in **Infrastructure.DependencyInjection**.
* Prefer injecting `IFusionCache` instead of `IMemoryCache`.
* Use `GetOrSetAsync` for *read‑through* caching; default absolute expiration ≤ 5 minutes unless data is static.
* Build cache keys as `<Layer>:<Entity>:<Id>` or `<Feature>:<Hash>` to avoid collisions.
* Never cache sensitive or per‑user data without proper key scoping (`user:{UserId}:...`).

## 8  EF Core & Identity Guidelines

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

## 9  AutoMapper Rules

* Profiles live in `src/Application/Common/Mapping`.
* Call `CreateMap<Source, Destination>().ReverseMap();` when bidirectional.
* Validate mapping config in tests with `configuration.AssertConfigurationIsValid();`.

---

## 10  Testing Matrix

| Target                      | Framework | Helper libs                 |
| --------------------------- | --------- | --------------------------- |
| Handlers & Services         | NUnit     | Moq · FluentAssertions      |
| Blazor Components           | bUnit     | Shouldly/FluentAssertions   |
| Integration (EF, Pipelines) | NUnit     | Testcontainers / InMemoryDb |

---

## 11  Migrators Conventions

* Each provider project (`Migrators.MSSQL`, etc.) holds **only migrations** plus provider‑specific DI.
* Migrations are chronological (`yyyymmddHHmm_description`).
* C# migration classes inherit `Migration` from **FluentMigrator**.
* Each migration **must be idempotent** and reversible (`Down()` implementation).

---

## 12  Commit Message Format

Follow **Conventional Commits**:

```
<type>(scope): <subject>
```

Types: `feat`, `fix`, `docs`, `refactor`, `test`, `build`, `chore`, `perf`, `ci`.

---

## 13  What Copilot **MUST NOT** Do

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

---

## 14  Blazor Development Best Practices

The guidelines below complement Sections 6 and 7, focusing on maintainability, performance, and user experience when building Blazor Server apps with MudBlazor.

1. **Keep Markup and Logic Together (optional)**  ⟶ It's acceptable to keep your component's logic within the same `.razor` file when this improves readability; separating into partial classes is optional.
2. **Keep Components Focused (≤ 300 LOC)**  ⟶ If a component grows, extract child components or view‑models to uphold the Single Responsibility Principle.
3. **Optimize Rendering with `@key`**  ⟶ Add `@key` when rendering collection items to improve diffing; use `Virtualize` only when essential.
4. **Prefer `Scoped` DI for UI State**  ⟶ Share per‑session data through scoped services (`SessionState`), avoiding singletons that could bleed state across users.
5. **Asynchronous Lifecycle Hooks**  ⟶ Use `async Task` versions of lifecycle methods (`OnInitializedAsync`, `OnParametersSetAsync`); inject `CancellationToken` for graceful tear‑down.
6. **Throttle `StateHasChanged`**  ⟶ Batch UI updates or debounce rapid events (e.g., keystrokes) to prevent unnecessary re‑renders.
7. **Dispose Correctly**  ⟶ Implement `IAsyncDisposable`/`IDisposable` for components or services that allocate unmanaged or JS interop resources, and unregister event handlers.
8. **Leverage MudBlazor Standard CSS**  ⟶ Favor MudBlazor's built-in classes and theme system; CSS isolation and BEM naming are optional.
9. **Centralize Themeing**  ⟶ Expose a single `MudTheme` in `MainLayout`/`App.razor` and use theme variables rather than hard‑coded MudBlazor parameters for brand consistency.
10. **Wrap JS Interop**  ⟶ Encapsulate JavaScript calls in C# extension methods (`IJSRuntime.InvokeMyPluginAsync(...)`) to avoid scattered string literals and enable unit testing via `IJSRuntime` mocks.
11. **Skip Prerendering**  ⟶ Do **not** enable `render-mode="ServerPrerendered"`; render components only after the realtime Blazor circuit is established to avoid known hydration issues.
12. **Error Handling with `ErrorBoundary`**  ⟶ Wrap pages in `<ErrorBoundary>` (or `MudErrorBoundary`) and log exceptions with `ILogger`; surface friendly error messages via MudBlazor `Snackbar`.
13. **Accessibility (WCAG 2.1 AA)**  ⟶ Provide `aria-*` attributes, keyboard navigation, and sufficient color contrast; verify using browser DevTools and tools like WAVE.
14. **Progressive Enhancement**  ⟶ Use feature flags or `IOptionsSnapshot` to roll out experimental components, allowing safe toggling in production.
