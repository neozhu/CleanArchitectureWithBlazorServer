# Migrate MediatR To Mediator Design

## Status

Approved for planning and implementation.

## Summary

Migrate the repository from `MediatR` to `martinothamar/Mediator` by deleting `MediatR` package and namespace dependencies rather than preserving a repository-owned MediatR-compatible facade.

This migration is still conservative about behavior, but it is no longer conservative about API shape:

- Application, domain, infrastructure, UI, and tests should move to `Mediator` / `Mediator.Abstractions` namespaces directly.
- `Mediator.Abstractions` should be referenced by projects that only need contracts.
- `Mediator.SourceGenerator` should be referenced only by the outer runtime layer that owns DI wiring and generated handler dispatch.
- Existing behaviors such as request handling, notification publishing, domain event dispatch, and scoped mediator usage should remain intact or be reconnected with equivalent semantics.

## Goals

- Remove direct `MediatR` references from source, tests, and project files.
- Move shared contracts to `Mediator.Abstractions`.
- Preserve current application behavior for request sending, notification publishing, and scoped mediator usage.
- Prepare the repository for a later runtime registration step that adds `Mediator.SourceGenerator` in the correct outer layer.

## Non-Goals

- Do not keep or expand the temporary repo-owned `MediatR` compatibility layer.
- Do not redesign the command/query architecture.
- Do not change feature behavior unless required by the mediator migration.
- Do not pull `Mediator.SourceGenerator` into low-level shared projects that only need abstractions.

## Current State

The repository currently contains three conflicting mediator shapes:

- Legacy source files still import `MediatR`.
- Temporary compatibility contracts exist under `src/Domain/Common/MediatorCompatibility/MediatR/*`.
- Migration safety-net tests already target low-level `Mediator` runtime concepts in a few places.

That mixed state is unstable. The next implementation step must simplify the contract layer instead of adding more compatibility code.

## Approved Architecture

Use direct `Mediator` abstractions throughout the codebase.

### Contract Layer

- `Domain` references `Mediator.Abstractions`.
- `Application` imports `Mediator` namespaces directly for requests, notifications, handlers, behaviors, preprocessors, and exception handlers.
- `DomainEvent` implements `Mediator.INotification` directly.
- UI and integration tests may resolve `Mediator.IMediator` directly once runtime wiring is in place.

### Runtime Layer

- `Infrastructure` will eventually own `Mediator.SourceGenerator` and registration.
- `Application/DependencyInjection.cs` must stop depending on `AddMediatR(...)`.
- `IScopedMediator` remains as a repository abstraction, but its implementation should resolve the real `Mediator.IMediator` service from a created scope.

### Temporary Transition Rule

Until the runtime registration task lands:

- Contract-only work may add `Mediator.Abstractions`.
- Temporary `MediatR` compatibility files should be removed instead of expanded.
- A minimal compile shim for `AddMediatR(...)` is allowed only as a short-lived bridge while the old DI registration still exists, but it must not become part of the final architecture.

## Behavioral Requirements

The migration must preserve or re-establish these behaviors:

- Existing request handlers still execute for current commands and queries.
- Existing notification handlers still execute for current notifications and domain events.
- `ParallelNoWaitPublisher` remains available and keeps its fire-and-forget behavior if the runtime layer still needs that strategy.
- `DispatchDomainEventsInterceptor` continues to publish domain events through `IScopedMediator`.
- `ScopedMediator` continues to resolve a fresh scoped `IMediator`.

## File Organization

Expected migration touchpoints:

- `src/Domain/Domain.csproj`
- `src/Application/_Imports.cs`
- `src/Application/DependencyInjection.cs`
- `src/Domain/Common/DomainEvent.cs`
- `src/Infrastructure/Services/MediatorWrapper/ScopedMediator.cs`
- `src/Server.UI/_Imports.razor`
- `src/Server.UI/Services/DialogServiceHelper.cs`
- `tests/Application.IntegrationTests/Testing.cs`
- `tests/Application.UnitTests/Common/MediatorCompatibility/*`
- `tests/Application.IntegrationTests/Common/MediatorCompatibility/*`

Files under `src/Domain/Common/MediatorCompatibility/MediatR/*` are temporary and should be deleted rather than treated as a stable design boundary.

## Migration Sequence

1. Keep the safety-net tests from Task 1.
2. Remove the temporary MediatR compatibility contracts.
3. Add `Mediator.Abstractions` where compile-time contracts are required.
4. Update source imports and contract usage to direct `Mediator` namespaces.
5. Keep DI compiling with the smallest temporary bridge necessary until runtime registration is replaced.
6. In a later task, add the real `Mediator.SourceGenerator` runtime registration in the outer layer and reconnect behaviors.
7. Remove any temporary DI shims before completion.

## Verification Scope

At minimum, verify:

- Contract-layer projects compile against `Mediator.Abstractions`.
- Safety-net tests that intentionally exercise `Mediator` low-level contracts compile in the expected direction.
- `IScopedMediator` still resolves `IMediator` from a created scope after runtime wiring is updated.
- There are no leftover `MediatR` package references or stable source dependencies outside temporary transitional code that is explicitly scheduled for deletion.

## Risks

- `Mediator` does not provide a drop-in equivalent for every MediatR interface, so request exception handling and DI registration need explicit redesign instead of namespace swapping.
- The temporary compatibility files can hide architectural drift if they are left in place.
- `AddMediatR(...)` currently anchors application startup, so contract-layer migration and runtime registration need to be staged carefully.

## Success Criteria

The migration is complete when:

- The repository no longer depends on `MediatR`.
- Shared code compiles against `Mediator.Abstractions` instead of `MediatR`.
- Runtime registration is owned by `Mediator.SourceGenerator` in the correct outer layer.
- Scoped mediator, notifications, and domain events still work.
- Documentation accurately describes the repository as using `Mediator`, not `MediatR`.
