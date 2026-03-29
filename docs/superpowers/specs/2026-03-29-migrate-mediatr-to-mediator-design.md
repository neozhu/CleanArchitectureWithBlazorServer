# Migrate MediatR To Mediator Design

## Status

Approved for planning.

## Summary

Migrate the repository from the `MediatR` package to `martinothamar/Mediator` while preserving the current repository-facing API shape and runtime behavior as closely as practical.

This migration is intentionally conservative:

- Keep existing request, notification, handler, and mediator usage patterns stable across `Application`, `Infrastructure`, `Server.UI`, and tests.
- Preserve pipeline behaviors, request preprocessors, request exception handlers, domain event publishing, and scoped mediator behavior.
- Isolate `Mediator` package details inside a compatibility layer instead of spreading them across feature code.

## Goals

- Replace the current MediatR runtime dependency with `martinothamar/Mediator`.
- Minimize churn in existing handlers, Razor pages, dialogs, and tests.
- Keep domain events and notification handlers working with equivalent semantics.
- Keep current cross-cutting behavior ordering and responsibilities intact.
- Make the migration safe for a template repository that downstream projects may follow.

## Non-Goals

- Do not redesign the command/query architecture.
- Do not split mediator usage into separate command/query/publisher abstractions in this change.
- Do not rewrite all handlers to use `Mediator` native interfaces directly.
- Do not do unrelated refactoring outside mediator migration touchpoints.

## Current State

The repository currently depends on MediatR semantics in several places:

- `Application` uses `IRequest<T>`, `IRequestHandler<,>`, `INotificationHandler<>`, `IPipelineBehavior<,>`, `IRequestPreProcessor<>`, and `IRequestExceptionHandler<,,>`.
- `Application/DependencyInjection.cs` registers MediatR handlers, behaviors, preprocessors, and a custom notification publisher.
- `Domain/Common/DomainEvent.cs` inherits from `INotification`.
- `Infrastructure` provides `IScopedMediator : IMediator` and a scoped wrapper implementation.
- `Server.UI` injects `IMediator` directly in Razor components.
- Integration tests resolve `IMediator` from DI and send requests through it.

## Proposed Architecture

Introduce an internal compatibility layer that preserves the current MediatR-like API shape used by the repository, while delegating execution to `martinothamar/Mediator`.

### Boundary

- Feature code continues to use familiar mediator abstractions.
- The compatibility layer becomes the only repository-wide mediator facade.
- `Mediator` becomes the execution engine behind that facade.
- Direct package-specific assumptions stay concentrated in registration and adapter code.

### Compatibility Layer Responsibilities

The compatibility layer should provide repository-local abstractions or aliases for:

- `IRequest<TResponse>`
- `IRequest`
- `INotification`
- `IMediator`
- `IRequestHandler<,>`
- `IRequestHandler<>`
- `INotificationHandler<>`
- `IPipelineBehavior<,>`
- `IRequestPreProcessor<>`
- `IRequestExceptionHandler<,,>`

This layer must make existing application code compile with minimal source changes.

### Runtime Adapter Responsibilities

Runtime adapters should:

- Resolve and dispatch requests through `Mediator`.
- Publish notifications through `Mediator`.
- Execute repository-defined behaviors and preprocessors in the expected order.
- Invoke registered request exception handlers using current result-oriented semantics.
- Support the existing scoped mediator behavior for domain event dispatch and other out-of-band publishing.

The runtime layer may also expose `global::Mediator.IMediator` in DI for low-level adapter wiring and migration-safety tests, but repository-facing application and UI code should continue to target the compatibility facade rather than the third-party `Mediator` namespace directly.

## DI And Registration Design

`Application/DependencyInjection.cs` should stop calling `AddMediatR(...)` and instead:

- Register `Mediator`.
- Register compatibility-layer abstractions and adapters.
- Register handlers from the application assembly through the new execution path.
- Register behaviors, preprocessors, and exception-handler bridges.
- Register the custom notification publishing strategy through the compatibility layer rather than via MediatR-specific configuration hooks.

`IScopedMediator` remains in place, but its implementation resolves the repository's compatibility `IMediator` facade rather than the external MediatR implementation.

## Behavioral Compatibility Strategy

### Requests And Handlers

- Existing request and handler types should remain structurally unchanged wherever possible.
- The migration should avoid touching feature-level business logic unless a direct compatibility issue forces a small edit.

### Pipeline Behaviors

The following existing behaviors remain active:

- `PerformanceBehaviour<,>`
- `FusionCacheBehaviour<,>`
- `CacheInvalidationBehaviour<,>`

The compatibility layer must preserve behavior ordering and execution position relative to handler invocation.

### Request Preprocessors

Current preprocessors such as `ValidationPreProcessor<TRequest>` and `LoggingPreProcessor<TRequest>` should be bridged into the new execution chain through a preprocessor adapter or equivalent behavior wrapper.

### Request Exception Handlers

Current `IRequestExceptionHandler<TRequest, TResponse, TException>` implementations should continue to translate exceptions into existing `Result`-style responses. If `Mediator` does not provide a directly equivalent hook, the compatibility layer must implement this behavior explicitly.

### Notifications And Domain Events

- `DomainEvent : INotification` remains conceptually unchanged.
- Existing notification handlers remain in place.
- `DispatchDomainEventsInterceptor` continues to publish events through `IScopedMediator`.
- Transaction-sensitive domain event publication semantics should remain consistent with current behavior.

### Notification Publishing Strategy

`ParallelNoWaitPublisher` is a migration risk because it depends on MediatR-specific notification publishing configuration.

The new design moves notification dispatch strategy into repository-owned compatibility code so behavior can be explicitly implemented instead of relying on a package-specific extension point.

## File Organization

Likely new or updated areas:

- `src/Application/Common/MediatorCompatibility/*`
- `src/Infrastructure/Services/MediatorCompatibility/*`
- `src/Application/DependencyInjection.cs`
- `src/Application/_Imports.cs`
- `src/Domain/Common/DomainEvent.cs`
- `src/Infrastructure/Services/MediatorWrapper/ScopedMediator.cs`
- `tests/Application.IntegrationTests/Testing.cs`
- `README.md`

Exact filenames may vary, but compatibility code should be centralized and not mixed into feature folders.

## Migration Steps

1. Work on branch `migrate-mediatr-to-mediator`.
2. Add `martinothamar/Mediator` package references where needed.
3. Remove direct `MediatR` package references once compatibility replacements are ready.
4. Introduce the repository compatibility abstractions.
5. Implement request, notification, and mediator runtime adapters.
6. Rewire DI registration away from `AddMediatR(...)`.
7. Reconnect behaviors, preprocessors, and exception handling.
8. Reconnect custom notification publishing strategy.
9. Update scoped mediator implementation to use the compatibility facade.
10. Fix compile breaks in UI, tests, and shared imports.
11. Update documentation that names MediatR as the backend mediator technology.
12. Run verification before claiming success.

## Verification Scope

At minimum, verify:

- Solution or affected projects compile successfully.
- `Application` request handling works for at least one command and one query.
- Notification publishing works for at least one existing notification flow.
- Validation preprocessing still runs before handler execution.
- Request exception handlers still convert exceptions to expected responses.
- Domain events still publish from `DispatchDomainEventsInterceptor`.
- `IScopedMediator` still works when publishing and sending from a scoped context.
- Integration tests that rely on `Testing.SendAsync(...)` continue to function or are updated with minimal API churn.

## Risks

### Highest Risks

- `ParallelNoWaitPublisher` may not have a direct analogue in `Mediator`.
- Open generic pipeline and exception-handler wiring may differ enough to require explicit bridging code.
- Preserving near-zero source churn while changing runtime semantics may expose edge-case mismatches during compilation or tests.

### Risk Mitigation

- Validate notification dispatch strategy early before broad refactoring.
- Prove one request, one notification, and one exception-handler path end to end before converting the whole registration pipeline.
- Keep the compatibility layer narrow and repository-owned to limit fallout.

## Success Criteria

The migration is complete when:

- The repository no longer depends on `MediatR` as its runtime mediator library.
- Existing application and UI mediator usage patterns remain substantially unchanged.
- Behaviors, preprocessors, exception handlers, notifications, domain events, and scoped mediator usage continue to work.
- Documentation reflects the new underlying mediator engine without misleading consumers about how the template behaves.

## Follow-Up

After this design is reviewed and accepted, the next step is to create a detailed implementation plan before any code migration begins.
