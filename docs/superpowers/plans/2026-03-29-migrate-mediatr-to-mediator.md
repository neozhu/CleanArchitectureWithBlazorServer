# Migrate MediatR To Mediator Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development. Execute one task at a time with spec review first, then code-quality review.

**Goal:** Remove `MediatR` references from the repository, move shared contracts to `Mediator.Abstractions`, and then reconnect runtime registration to `Mediator.SourceGenerator` in the outer layer.

**Architecture:** This plan no longer preserves a repo-owned `MediatR` facade. Shared projects should reference `Mediator.Abstractions` directly. Temporary compatibility files created during earlier migration attempts must be deleted. Runtime registration belongs in infrastructure or another outer executable-facing layer, not in the shared contract layer.

**Tech Stack:** .NET 10, Blazor Server, NUnit, FluentAssertions, `Mediator.Abstractions`, `Mediator.SourceGenerator`, Microsoft DI

---

## Task 1: Add Migration Safety-Net Tests

Status: completed on branch `migrate-mediatr-to-mediator-main-impl`.

Keep the existing safety-net tests. They intentionally prove that low-level `Mediator` contracts are not fully wired yet.

## Task 2: Replace MediatR Contracts With Direct Mediator.Abstractions Usage

**Intent:** Delete the temporary MediatR compatibility layer and move the shared contract surface to `Mediator.Abstractions` without yet completing the runtime registration rewrite.

**Files:**
- Delete: `src/Domain/Common/MediatorCompatibility/MediatR/Contracts.cs`
- Delete: `src/Domain/Common/MediatorCompatibility/MediatR/Handlers.cs`
- Delete: `src/Domain/Common/MediatorCompatibility/MediatR/PipelineContracts.cs`
- Delete: `src/Domain/Common/MediatorCompatibility/MediatR/PreProcessorContracts.cs`
- Delete: `src/Domain/Common/MediatorCompatibility/MediatR/ExceptionContracts.cs`
- Delete: `src/Domain/Common/MediatorCompatibility/MediatR/NotificationPublishing.cs`
- Delete: `src/Domain/Common/MediatorCompatibility/MediatR/ServiceCollectionContracts.cs`
- Modify: `src/Domain/Domain.csproj`
- Modify: `src/Application/_Imports.cs`
- Modify: `src/Domain/Common/DomainEvent.cs`
- Modify: `tests/Application.UnitTests/Common/MediatorCompatibility/RequestExceptionHandlerStateTests.cs`

- [ ] **Step 1: Write the failing direct-abstractions smoke test**

Rewrite `tests/Application.UnitTests/Common/MediatorCompatibility/RequestExceptionHandlerStateTests.cs` into a direct `Mediator.Abstractions` smoke test. It should prove the project compiles against `Mediator.INotification`, `Mediator.IRequest<TResponse>`, and `Mediator.IPipelineBehavior<TRequest, TResponse>` rather than the deleted MediatR compatibility state type.

Suggested shape:

```csharp
using Mediator;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.MediatorCompatibility;

public class MediatorAbstractionsSmokeTests
{
    [Test]
    public void Contracts_ShouldCompileAgainstMediatorAbstractions()
    {
        _ = new SmokeNotification();
        _ = new SmokeRequest();
        _ = typeof(IPipelineBehavior<SmokeRequest, string>);
    }

    private sealed record SmokeNotification : INotification;
    private sealed record SmokeRequest : IRequest<string>;
}
```

- [ ] **Step 2: Run the smoke test to confirm it fails**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~MediatorAbstractionsSmokeTests" -v minimal`

Expected: FAIL because `Mediator.Abstractions` is not referenced yet.

- [ ] **Step 3: Add direct Mediator.Abstractions package support**

Update `src/Domain/Domain.csproj` to reference `Mediator.Abstractions`.

Keep the dependency in the lowest shared layer that exposes `DomainEvent` and shared mediator contracts.

- [ ] **Step 4: Remove the temporary MediatR compatibility files**

Delete all files under `src/Domain/Common/MediatorCompatibility/MediatR/`.

Do not replace them with new repo-owned MediatR shims.

- [ ] **Step 5: Update shared imports and domain event contracts**

Apply the smallest source edits needed so the shared code targets `Mediator` directly:

- `src/Application/_Imports.cs`: replace `global using MediatR;` and `global using MediatR.Pipeline;` with direct `Mediator` imports.
- `src/Domain/Common/DomainEvent.cs`: replace `global::MediatR` usage with `global::Mediator`.

- [ ] **Step 6: Add the temporary AddMediatR compile shim**

Because the runtime registration rewrite has not happened yet, keep `src/Application/DependencyInjection.cs` compiling by adding the smallest temporary `AddMediatR(...)` shim needed for this stage.

Constraints:

- The shim is transitional only.
- It must not recreate the deleted MediatR contract surface.
- It should exist only to let Task 2 land while Task 3 still owns the real DI rewrite.

- [ ] **Step 7: Re-run the smoke test**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~MediatorAbstractionsSmokeTests" -v minimal`

Expected: PASS, or at worst fail for a known unrelated baseline issue that already existed before this migration work.

- [ ] **Step 8: Run the scoped mediator safety-net test**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~ScopedMediatorTests" -v minimal`

Expected: The test should move closer to green by compiling against real `Mediator` abstractions. If it still fails, the remaining failure should be due to runtime wiring, not missing MediatR contracts.

- [ ] **Step 9: Commit**

```bash
git add src/Domain/Domain.csproj src/Application/_Imports.cs src/Domain/Common/DomainEvent.cs tests/Application.UnitTests/Common/MediatorCompatibility/RequestExceptionHandlerStateTests.cs src/Application/DependencyInjection.cs
git rm -r src/Domain/Common/MediatorCompatibility/MediatR
git commit -m "refactor: move mediator contracts to abstractions"
```

## Task 3: Replace DI Registration With Mediator.SourceGenerator Runtime Wiring

**Intent:** Remove the temporary `AddMediatR(...)` bridge, register the real `Mediator` runtime in the outer layer, and reconnect handlers, notifications, behaviors, and scoped mediator execution.

**Files:**
- Modify: `src/Application/DependencyInjection.cs`
- Modify: `src/Infrastructure/DependencyInjection.cs`
- Modify: `src/Infrastructure/Infrastructure.csproj`
- Modify: `src/Infrastructure/Services/MediatorWrapper/ScopedMediator.cs`
- Modify: `tests/Application.IntegrationTests/Testing.cs`
- Modify: `tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs`
- Modify: `tests/Application.UnitTests/Common/MediatorCompatibility/ParallelNoWaitPublisherTests.cs`
- Modify: `tests/Application.UnitTests/Infrastructure/MediatorCompatibility/ScopedMediatorTests.cs`

Notes:

- Add `Mediator.SourceGenerator` in the outer layer only.
- Delete the temporary `AddMediatR(...)` shim as part of this task.
- Reconnect `ParallelNoWaitPublisher`, scoped mediator resolution, and at least one existing query path through DI.
- Decide explicitly how current exception-handler behavior maps to `Mediator`. Do not assume a drop-in equivalent exists.

## Task 4: Update UI/Test Imports And Resolve Remaining Runtime Gaps

**Files:**
- Modify: `src/Server.UI/_Imports.razor`
- Modify: `src/Server.UI/Services/DialogServiceHelper.cs`
- Modify: `tests/Application.IntegrationTests/Testing.cs`
- Modify: any source or test files still importing `MediatR`

Notes:

- Remove leftover `MediatR` namespace imports after runtime wiring is stable.
- Keep `IScopedMediator` intact as a repository abstraction if it still provides value.

## Task 5: Remove Leftover MediatR References And Update Documentation

**Files:**
- Modify: `README.md`
- Modify: `src/Server.UI/Pages/Public/Index.razor`
- Modify: `src/Server.UI/Pages/AI/Chatbot.razor`
- Modify: any `.csproj` files still referencing `MediatR`

Verification:

- `rg -n "MediatR" src tests README.md`
- Update user-facing copy so the app no longer claims it uses MediatR.

## Final Verification

Before claiming the migration complete, run:

- `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj -v minimal`
- `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~MediatorCompatibilityTests" -v minimal`
- `rg -n "MediatR" src tests README.md`

If any command fails, report the exact remaining blocker instead of declaring success.
