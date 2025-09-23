## OneOf Refactor Discussion and Plan (2025-09-20)

### Context
- Project: CleanArchitectureWithBlazorServer
- Current result pattern: `Result` / `Result<T>` with `Succeeded`, `Errors` (strings), `Match/Map/Bind` helpers.
- UI usages: `Products.razor`, `ProductFormDialog.razor` consume `Result/Result<T>`.
- Exception handling: MediatR `IRequestExceptionHandler` implementations for:
  - `DbExceptionHandler<TRequest, TResponse, TException>`
  - `ValidationExceptionHandler<TRequest, TResponse, TException>`
  - `NotFoundExceptionHandler<TRequest, TResponse, TException>`
  - `FallbackExceptionHandler<TRequest, TResponse, TException>`
- Goal: Adopt `OneOf`-style discriminated unions to enable strong, exhaustive, multi-branch results without pervasive try/catch in handlers.

### Problems Identified
- `Result` is binary (success/failure) and errors are plain strings, limiting type-safety and exhaustiveness.
- `Result.Failure` signatures currently using `params IEnumerable<string>` conflict with handlers that reflect for `Failure(string[])`.
- Handlers must branch on strings instead of strong error types.
- Try/catch is used (or would be needed) to map EF exceptions; we want interceptor-based mapping.

### Design Direction
Adopt `OneOf` with a pair of reusable, generic result types for the whole application:
- `AppResult` (no payload)
- `AppResult<T>` (payload)

And a small, reusable set of error union cases:
- `Success` (marker success)
- `ValidationFailed` (collection of messages)
- `NotFound`
- `Conflict` (unique constraint, duplicates, business conflict)
- `HasDependents` (FK reference prevents deletion)
- `Unexpected` (fallback)

This avoids defining a distinct `XxxResult` per entity/feature, while preserving strong typing and exhaustive matching.

### Phased Plan
1) Stabilize current `Result` (backward-compatible)
   - Change method signatures to align with existing exception handlers' reflection:
     - `public static Result Failure(params string[] errors)`
     - `public static Result<T> Failure(params string[] errors)`
     - And corresponding `FailureAsync(params string[] errors)` methods.
   - Optional convenience additions:
     - `Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure)`
     - `Switch(Action onSuccess, Action<string> onFailure)`
     - `TryPickSuccess(out T)` / `TryPickFailure(out IReadOnlyList<string>)` for `Result<T>`.

2) Introduce OneOf
   - Add NuGet packages in Application project:
     - `OneOf`
     - `OneOf.SourceGenerator` (optional but recommended)
   - Define common error types in `Application/Common/Errors/`:
     ```csharp
     public readonly record struct Success;
     public readonly record struct NotFound(string Message);
     public readonly record struct ValidationFailed(IReadOnlyList<string> Messages);
     public readonly record struct Conflict(string Message);
     public readonly record struct HasDependents(string Message);
     public readonly record struct Unexpected(string Message);
     ```
   - Define reusable result unions in `Application/Common/Results/`:
     ```csharp
     [GenerateOneOf]
     public partial class AppResult : OneOfBase<
         Success, ValidationFailed, NotFound, Conflict, HasDependents, Unexpected> { }

     [GenerateOneOf]
     public partial class AppResult<T> : OneOfBase<
         T, ValidationFailed, NotFound, Conflict, HasDependents, Unexpected> { }
     ```

3) OneOf-based Exception Handlers (no try/catch in handlers)
   - Add parallel MediatR exception handlers targeting `AppResult` / `AppResult<T>`:
     - Validation → `new ValidationFailed(errors)`
     - NotFound → `new NotFound(message)`
     - DbUpdateException family → `new Conflict(...)`, `new HasDependents(...)`, or `new ValidationFailed(...)` as appropriate
     - Fallback → `new Unexpected(ex.Message)`
   - Keep existing `IResult`-based handlers for legacy `Result` usage.
   - Register both; MediatR will pick handlers based on `TResponse`.

4) Pilot Migration (Products feature)
   - Change handler signatures to return `AppResult<T>` / `AppResult`.
   - Remove explicit try/catch in handlers; rely on OneOf exception handlers.
   - Update `ProductFormDialog.razor` to exhaustively match OneOf results:
     ```csharp
     AppResult<int> r = await Mediator.Send(_model);
     r.Match(
       id => { MudDialog.Close(DialogResult.Ok(true)); Snackbar.Add(ConstantString.SaveSuccess, Severity.Info); },
       v  => Snackbar.Add(string.Join("\n", v.Messages), Severity.Error),
       nf => Snackbar.Add(nf.Message, Severity.Error),
       c  => Snackbar.Add(c.Message, Severity.Error),
       hd => Snackbar.Add(hd.Message, Severity.Error),
       u  => Snackbar.Add(u.Message, Severity.Error)
     );
     ```
   - Keep `Products.razor` export/query paths on `Result<byte[]>` initially; migrate later.

5) Gradual Rollout
   - Adopt `AppResult` / `AppResult<T>` across other features incrementally.
   - Optionally provide extension bridge methods to convert between `Result<T>` and `AppResult<T>` during transition.

### File References (current)
- `src/Application/Common/Models/Result.cs`
- `src/Application/Common/Interfaces/IResult.cs`
- `src/Application/Common/ExceptionHandlers/DbExceptionHandler.cs`
- `src/Application/Common/ExceptionHandlers/ValidationExceptionHandler.cs`
- `src/Application/Common/ExceptionHandlers/NotFoundExceptionHandler.cs`
- `src/Application/Common/ExceptionHandlers/FallbackExceptionHandler.cs`
- `src/Application/Features/Products/Commands/AddEdit/AddEditProductCommand.cs`
- `src/Application/Features/Products/Commands/Delete/DeleteProductCommand.cs`
- `src/Server.UI/Pages/Products/Products.razor`
- `src/Server.UI/Pages/Products/Components/ProductFormDialog.razor`

### Notes and Rationale
- Using two generic OneOf-based result types allows reuse across all entities without creating per-entity result classes.
- Strongly-typed error cases improve readability, localization, and testability.
- Exhaustive matching in UI enforces handling of all branches, avoiding silent fallbacks.
- OneOf exception handlers eliminate repetitive try/catch in handlers, centralizing mapping of EF and domain exceptions.

### Next Actions (Proposed)
1) Update `Result.cs` failure signatures to `params string[]` and add convenience APIs.
2) Add `OneOf` packages and define `AppResult` / `AppResult<T>` and common error types.
3) Implement OneOf-based MediatR exception handlers for `AppResult` / `AppResult<T>`.
4) Pilot migrate Products feature (handlers + `ProductFormDialog.razor`).
5) Roll out to other features.

---

This document captures the discussion and actionable plan for adopting OneOf while maintaining backward compatibility and leveraging MediatR exception handling to avoid per-handler try/catch blocks.



