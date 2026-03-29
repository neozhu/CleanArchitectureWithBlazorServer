# Migrate MediatR To Mediator Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the repository's MediatR runtime dependency with `martinothamar/Mediator` while preserving the current request, notification, pipeline, exception-handler, domain-event, and scoped-mediator behavior with minimal source churn.

**Architecture:** Keep the repository-facing `MediatR` API shape by introducing repo-owned compatibility contracts in the `MediatR` and `MediatR.Pipeline` namespaces inside the lowest shared layer that both `Domain` and `Application` can consume, then back those contracts with a `Mediator`-powered runtime adapter registered through DI. Concentrate package-specific code in compatibility and registration files so feature handlers, Razor pages, and tests mostly stay unchanged.

**Tech Stack:** .NET 10, Blazor Server, NUnit, FluentAssertions, `martinothamar/Mediator`, Microsoft DI

---

## File Map

### Existing files to modify

- `src/Application/DependencyInjection.cs`
  Remove `AddMediatR(...)` and replace it with package-agnostic registrations for behaviors, preprocessors, exception handlers, and an application-assembly marker that infrastructure can consume.
- `src/Application/_Imports.cs`
  Remove package-owned MediatR globals if needed and ensure compatibility namespaces remain available.
- `src/Application/Common/PublishStrategies/ParallelNoWaitPublisher.cs`
  Keep the public intent the same while switching to repo-owned notification executor abstractions.
- `src/Application/ApplicationAssemblyMarker.cs`
  Provide a stable assembly marker so infrastructure can register application handlers without creating a reverse dependency.
- `src/Domain/Common/DomainEvent.cs`
  Keep `DomainEvent : INotification` semantics using repo-owned compatibility contracts.
- `src/Domain/Domain.csproj`
  Replace the `MediatR` package reference with whatever compile-time package support the shared compatibility contracts require.
- `src/Infrastructure/DependencyInjection.cs`
  Keep `IScopedMediator` registration and add the `Mediator` runtime plus compatibility-layer service registrations required by the new execution path.
- `src/Infrastructure/Services/MediatorWrapper/ScopedMediator.cs`
  Resolve the repo-owned compatibility `IMediator` facade instead of the external MediatR runtime service.
- `src/Server.UI/_Imports.razor`
  Preserve `@inject IMediator Mediator` while pointing imports at repo-owned compatibility types.
- `src/Server.UI/Services/DialogServiceHelper.cs`
  Keep request signatures compiling against repo-owned `IRequest<T>`.
- `tests/Application.IntegrationTests/Testing.cs`
  Keep `SendAsync` and service resolution working against repo-owned `IMediator`.
- `README.md`
  Update stack documentation and migration-sensitive workflow text that currently names MediatR.
- `src/Server.UI/Pages/Public/Index.razor`
  Update the public tech pill from `MediatR` to `Mediator` or a compatibility-safe label.
- `src/Server.UI/Pages/AI/Chatbot.razor`
  Update the architecture description string so it no longer claims the app uses MediatR.

### New files to create

- `src/Domain/Common/MediatorCompatibility/MediatR/Contracts.cs`
  Define repo-owned `IBaseRequest`, `IRequest`, `IRequest<TResponse>`, `INotification`, and `IMediator` in the `MediatR` namespace.
- `src/Domain/Common/MediatorCompatibility/MediatR/Handlers.cs`
  Define repo-owned `IRequestHandler`, `INotificationHandler`, and request-handler delegate shapes in the `MediatR` namespace.
- `src/Domain/Common/MediatorCompatibility/MediatR/PipelineContracts.cs`
  Define repo-owned `IPipelineBehavior<TRequest, TResponse>` and `RequestHandlerDelegate<TResponse>`.
- `src/Domain/Common/MediatorCompatibility/MediatR/PreProcessorContracts.cs`
  Define repo-owned `IRequestPreProcessor<TRequest>` in the `MediatR.Pipeline` namespace.
- `src/Domain/Common/MediatorCompatibility/MediatR/ExceptionContracts.cs`
  Define repo-owned `IRequestExceptionHandler<TRequest, TResponse, TException>` and `RequestExceptionHandlerState<TResponse>`.
- `src/Domain/Common/MediatorCompatibility/MediatR/NotificationPublishing.cs`
  Define repo-owned `INotificationPublisher` and `NotificationHandlerExecutor`.
- `src/Infrastructure/Services/MediatorCompatibility/MediatorFacade.cs`
  Implement the repo-owned `IMediator` facade on top of the `Mediator` runtime.
- `src/Infrastructure/Services/MediatorCompatibility/CompatibilityRequestPipeline.cs`
  Execute preprocessors, behaviors, handler invocation, and exception-handler dispatch in the current repository order.
- `src/Infrastructure/Services/MediatorCompatibility/CompatibilityNotificationPublisher.cs`
  Resolve notification handlers and execute them through the configured repo-owned `INotificationPublisher`.
- `src/Infrastructure/Services/MediatorCompatibility/CompatibilityServiceCollectionExtensions.cs`
  Hold the DI wiring that registers compatibility abstractions, runtime adapters, and `Mediator`.
- `tests/Application.UnitTests/Common/MediatorCompatibility/ParallelNoWaitPublisherTests.cs`
  Lock the custom publisher's fire-and-forget semantics.
- `tests/Application.UnitTests/Infrastructure/MediatorCompatibility/ScopedMediatorTests.cs`
  Lock `IScopedMediator` behavior against the repo-owned `IMediator`.
- `tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs`
  Verify `IMediator` and `IScopedMediator` can resolve and send existing requests through DI after migration.

## Task 1: Add Migration Safety-Net Tests

**Files:**
- Create: `tests/Application.UnitTests/Common/MediatorCompatibility/ParallelNoWaitPublisherTests.cs`
- Create: `tests/Application.UnitTests/Infrastructure/MediatorCompatibility/ScopedMediatorTests.cs`
- Create: `tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs`
- Modify: `tests/Application.IntegrationTests/Testing.cs`

- [ ] **Step 1: Write the failing publisher test**

```csharp
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.MediatorCompatibility;

public class ParallelNoWaitPublisherTests
{
    [Test]
    public async Task Publish_ShouldReturnBeforeHandlersComplete()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var unblock = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var publisher = new ParallelNoWaitPublisher();
        var executors = new[]
        {
            new NotificationHandlerExecutor(
                (_, _) =>
                {
                    started.TrySetResult();
                    return unblock.Task;
                })
        };

        var publishTask = publisher.Publish(executors, new TestNotification(), CancellationToken.None);

        await started.Task;
        publishTask.IsCompletedSuccessfully.Should().BeTrue();

        unblock.TrySetResult();
    }

    private sealed record TestNotification() : INotification;
}
```

- [ ] **Step 2: Run the unit test to confirm it fails**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~ParallelNoWaitPublisherTests" -v minimal`
Expected: FAIL because `NotificationHandlerExecutor` and `INotification` compatibility types do not exist yet.

- [ ] **Step 3: Write the failing scoped mediator test**

```csharp
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MediatorWrapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Infrastructure.MediatorCompatibility;

public class ScopedMediatorTests
{
    [Test]
    public async Task Send_ShouldResolveMediatorFromCreatedScope()
    {
        var services = new ServiceCollection();
        services.AddScoped<IMediator, FakeMediator>();

        var provider = services.BuildServiceProvider();
        var scopedMediator = new ScopedMediator(provider.GetRequiredService<IServiceScopeFactory>());

        var response = await scopedMediator.Send(new PingQuery(), CancellationToken.None);

        response.Should().Be("pong");
    }

    private sealed record PingQuery() : IRequest<string>;

    private sealed class FakeMediator : IMediator
    {
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            => Task.FromResult((TResponse)(object)"pong");

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
            => Task.CompletedTask;

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
            => Task.FromResult<object?>("pong");

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            => Task.CompletedTask;

        public Task Publish(object notification, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            => AsyncEnumerable.Empty<TResponse>();

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
            => AsyncEnumerable.Empty<object?>();
    }
}
```

- [ ] **Step 4: Run the scoped mediator test to confirm it fails**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~ScopedMediatorTests" -v minimal`
Expected: FAIL because the repo-owned compatibility `IMediator`, `IRequest<T>`, and `IStreamRequest<T>` contracts do not exist yet.

- [ ] **Step 5: Write the failing integration test**

```csharp
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MediatorWrapper;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Common.MediatorCompatibility;

public class MediatorCompatibilityTests : TestBase
{
    [Test]
    public async Task ShouldResolveMediatorAndSendExistingQuery()
    {
        using var scope = Testing.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GetAllProductsQuery());

        result.Should().NotBeNull();
    }

    [Test]
    public async Task ShouldResolveScopedMediatorAndSendExistingQuery()
    {
        using var scope = Testing.CreateScope();
        var scopedMediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();

        var result = await scopedMediator.Send(new GetAllProductsQuery());

        result.Should().NotBeNull();
    }
}
```

- [ ] **Step 6: Add the smallest helper needed in `Testing.cs`**

```csharp
public static IServiceScope CreateScope()
{
    return _scopeFactory.CreateScope();
}
```

- [ ] **Step 7: Run the integration test to confirm it fails**

Run: `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~MediatorCompatibilityTests" -v minimal`
Expected: FAIL because the container does not yet expose the repo-owned compatibility `IMediator`.

- [ ] **Step 8: Commit**

```bash
git add tests/Application.UnitTests/Common/MediatorCompatibility/ParallelNoWaitPublisherTests.cs tests/Application.UnitTests/Infrastructure/MediatorCompatibility/ScopedMediatorTests.cs tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs tests/Application.IntegrationTests/Testing.cs
git commit -m "test: add mediator migration safety nets"
```

## Task 2: Introduce Repo-Owned MediatR Compatibility Contracts

**Files:**
- Create: `src/Domain/Common/MediatorCompatibility/MediatR/Contracts.cs`
- Create: `src/Domain/Common/MediatorCompatibility/MediatR/Handlers.cs`
- Create: `src/Domain/Common/MediatorCompatibility/MediatR/PipelineContracts.cs`
- Create: `src/Domain/Common/MediatorCompatibility/MediatR/PreProcessorContracts.cs`
- Create: `src/Domain/Common/MediatorCompatibility/MediatR/ExceptionContracts.cs`
- Create: `src/Domain/Common/MediatorCompatibility/MediatR/NotificationPublishing.cs`
- Modify: `src/Application/_Imports.cs`
- Modify: `src/Domain/Common/DomainEvent.cs`
- Modify: `src/Domain/Domain.csproj`

- [ ] **Step 1: Write the failing contract smoke test**

Add assertions to `tests/Application.UnitTests/Common/MediatorCompatibility/ParallelNoWaitPublisherTests.cs` or a dedicated new contract test that references `INotification`, `IRequest<T>`, and `RequestExceptionHandlerState<TResponse>`.

```csharp
[Test]
public void RequestExceptionHandlerState_ShouldStoreHandledResponse()
{
    var state = new RequestExceptionHandlerState<string>();
    state.SetHandled("ok");

    state.Handled.Should().BeTrue();
    state.Response.Should().Be("ok");
}
```

- [ ] **Step 2: Run the smoke test to confirm it fails**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~RequestExceptionHandlerState" -v minimal`
Expected: FAIL because the compatibility contracts do not exist yet.

- [ ] **Step 3: Implement the repo-owned contracts in the shared compatibility layer**

Create the contracts in `src/Domain/Common/MediatorCompatibility/MediatR/*` so both `Domain` and `Application` can compile against them without a reverse project reference.

```csharp
namespace MediatR;

public interface IBaseRequest { }

public interface IRequest : IBaseRequest { }

public interface IRequest<out TResponse> : IBaseRequest { }

public interface INotification { }

public interface IStreamRequest<out TResponse> : IBaseRequest { }

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
    Task<object?> Send(object request, CancellationToken cancellationToken = default);
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
    Task Publish(object notification, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default);
}
```

- [ ] **Step 4: Implement handler, pipeline, preprocessor, exception, and notification-publishing contracts**

```csharp
namespace MediatR;

public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}

public sealed class RequestExceptionHandlerState<TResponse>
{
    public bool Handled { get; private set; }
    public TResponse? Response { get; private set; }

    public void SetHandled(TResponse response)
    {
        Handled = true;
        Response = response;
    }
}
```

```csharp
namespace MediatR.Pipeline;

public interface IRequestPreProcessor<in TRequest> where TRequest : notnull
{
    Task Process(TRequest request, CancellationToken cancellationToken);
}
```

- [ ] **Step 5: Keep imports stable**

Update `src/Application/_Imports.cs` to stop depending on the external package-only globals while leaving `global using MediatR;` and `global using MediatR.Pipeline;` intact so feature code still compiles against the repo-owned contracts.

- [ ] **Step 6: Keep `DomainEvent` unchanged in shape**

```csharp
namespace CleanArchitecture.Blazor.Domain.Common;

public abstract class DomainEvent : INotification
{
    protected DomainEvent()
    {
        DateOccurred = DateTimeOffset.UtcNow;
    }

    public bool IsPublished { get; set; }
    public DateTimeOffset DateOccurred { get; protected set; }
}
```

- [ ] **Step 7: Run the unit tests**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~MediatorCompatibility" -v minimal`
Expected: PASS for the contract-focused tests; compile may still fail elsewhere until runtime adapters exist.

- [ ] **Step 8: Keep `Domain.csproj` aligned with the shared contracts**

If the shared compatibility contracts must inherit from the `Mediator` package's request/notification interfaces for source generation to work, add that package reference here and remove the old `MediatR` reference. If standalone repo-owned contracts compile cleanly, keep `Domain.csproj` free of any mediator-package dependency.

- [ ] **Step 9: Commit**

```bash
git add src/Domain/Common/MediatorCompatibility/MediatR/Contracts.cs src/Domain/Common/MediatorCompatibility/MediatR/Handlers.cs src/Domain/Common/MediatorCompatibility/MediatR/PipelineContracts.cs src/Domain/Common/MediatorCompatibility/MediatR/PreProcessorContracts.cs src/Domain/Common/MediatorCompatibility/MediatR/ExceptionContracts.cs src/Domain/Common/MediatorCompatibility/MediatR/NotificationPublishing.cs src/Application/_Imports.cs src/Domain/Common/DomainEvent.cs src/Domain/Domain.csproj
git commit -m "feat: add mediator compatibility contracts"
```

## Task 3: Implement the Mediator-Powered Runtime Facade

**Files:**
- Create: `src/Infrastructure/Services/MediatorCompatibility/MediatorFacade.cs`
- Create: `src/Infrastructure/Services/MediatorCompatibility/CompatibilityServiceCollectionExtensions.cs`
- Create: `src/Application/ApplicationAssemblyMarker.cs`
- Modify: `src/Application/DependencyInjection.cs`
- Modify: `src/Infrastructure/DependencyInjection.cs`
- Modify: `src/Infrastructure/Infrastructure.csproj`
- Modify: `src/Domain/Domain.csproj`

- [ ] **Step 1: Write the failing DI smoke test**

Extend `tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs` to verify the DI container resolves `IMediator` before any real request is sent.

```csharp
[Test]
public void ShouldResolveCompatibilityMediator()
{
    using var scope = Testing.CreateScope();
    FluentActions.Invoking(() => scope.ServiceProvider.GetRequiredService<IMediator>())
        .Should().NotThrow();
}
```

- [ ] **Step 2: Run the integration test to confirm it fails**

Run: `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~ShouldResolveCompatibilityMediator" -v minimal`
Expected: FAIL because no compatibility `IMediator` service is registered.

- [ ] **Step 3: Add the `Mediator` package and runtime facade**

Add the `Mediator` NuGet package to `src/Infrastructure/Infrastructure.csproj` using the latest stable version approved for the repo's target framework at implementation time.

```csharp
namespace CleanArchitecture.Blazor.Infrastructure.Services.MediatorCompatibility;

internal sealed class MediatorFacade : IMediator
{
    private readonly global::Mediator.IMediator _innerMediator;
    private readonly CompatibilityNotificationPublisher _notificationPublisher;

    public MediatorFacade(global::Mediator.IMediator innerMediator, CompatibilityNotificationPublisher notificationPublisher)
    {
        _innerMediator = innerMediator;
        _notificationPublisher = notificationPublisher;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        => _innerMediator.Send(request, cancellationToken);

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
        => _notificationPublisher.Publish(notification, cancellationToken);
}
```

If the shared compatibility contracts cannot directly inherit from `Mediator` package contracts, move the type translation into `MediatorFacade` or a nearby adapter rather than editing feature requests and handlers.

- [ ] **Step 4: Move registration out of `AddMediatR(...)`**

Split responsibilities by layer:

- `src/Application/DependencyInjection.cs` registers validators, preprocessors, behaviors, exception handlers, and `UserProfileStateService`, but does not reference the external `Mediator` package.
- `src/Application/ApplicationAssemblyMarker.cs` exposes a marker type from the application assembly.
- `src/Infrastructure/DependencyInjection.cs` calls the compatibility runtime registration and passes `typeof(ApplicationAssemblyMarker).Assembly`.

Use package-agnostic registrations in application:

```csharp
services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(DbExceptionHandler<,,>));
services.AddTransient(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FusionCacheBehaviour<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehaviour<,>));
```

and runtime registration in infrastructure:

```csharp
services.AddCompatibilityMediator(typeof(ApplicationAssemblyMarker).Assembly, options =>
{
    options.NotificationPublisherType = typeof(ParallelNoWaitPublisher);
});
```

- [ ] **Step 5: Add the `Mediator` package to the correct project set and stop compiling against MediatR**

Remove:

```xml
<PackageReference Include="MediatR" Version="14.1.0" />
```

from `src/Domain/Domain.csproj`. Add `Mediator` to `src/Infrastructure/Infrastructure.csproj`, and add it to `src/Domain/Domain.csproj` only if the shared compatibility contracts need to inherit from the package's contracts for source generation or dispatch interop.

- [ ] **Step 6: Run the integration tests**

Run: `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~MediatorCompatibilityTests" -v minimal`
Expected: FAIL later in the request pipeline until behaviors and publishers are bridged, but DI resolution should now succeed.

- [ ] **Step 7: Commit**

```bash
git add src/Infrastructure/Services/MediatorCompatibility/MediatorFacade.cs src/Infrastructure/Services/MediatorCompatibility/CompatibilityServiceCollectionExtensions.cs src/Application/ApplicationAssemblyMarker.cs src/Application/DependencyInjection.cs src/Infrastructure/DependencyInjection.cs src/Infrastructure/Infrastructure.csproj src/Domain/Domain.csproj tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs
git commit -m "feat: register mediator compatibility runtime"
```

## Task 4: Bridge Behaviors, Preprocessors, and Exception Handlers

**Files:**
- Create: `src/Infrastructure/Services/MediatorCompatibility/CompatibilityRequestPipeline.cs`
- Modify: `src/Application/DependencyInjection.cs`
- Modify: `src/Application/Common/PublishStrategies/ParallelNoWaitPublisher.cs`

- [ ] **Step 1: Write the failing behavior and exception-path tests**

Add coverage in `tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs` for:

- validation failing before handler execution
- an existing `Result<int>` request receiving translated error output from an exception handler

```csharp
[Test]
public async Task ShouldApplyValidationBeforeHandlerExecution()
{
    var invalidCommand = new AddEditProductCommand { Name = string.Empty };

    var result = await SendAsync(invalidCommand);

    result.Succeeded.Should().BeFalse();
}
```

- [ ] **Step 2: Run the integration tests to confirm they fail**

Run: `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~MediatorCompatibilityTests" -v minimal`
Expected: FAIL because the new runtime path does not yet execute preprocessors and exception handlers.

- [ ] **Step 3: Implement the compatibility request pipeline**

```csharp
internal sealed class CompatibilityRequestPipeline
{
    public async Task<TResponse> Execute<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> handler)
        where TRequest : IRequest<TResponse>
    {
        foreach (var preProcessor in _preProcessors)
            await preProcessor.Process(request, cancellationToken);

        RequestHandlerDelegate<TResponse> next = handler;
        foreach (var behavior in _behaviors.Reverse())
        {
            var current = next;
            next = () => behavior.Handle(request, current, cancellationToken);
        }

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            return await _exceptionProcessor.TryHandle<TRequest, TResponse>(request, ex, cancellationToken);
        }
    }
}
```

- [ ] **Step 4: Rewire `ParallelNoWaitPublisher` to the repo-owned contracts**

Keep the fire-and-forget behavior:

```csharp
public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
{
    foreach (var handler in handlerExecutors)
        Task.Run(() => handler.HandlerCallback(notification, cancellationToken));

    return Task.CompletedTask;
}
```

- [ ] **Step 5: Register behaviors, preprocessors, and exception handlers explicitly**

Keep the current ordering from `src/Application/DependencyInjection.cs`:

1. request preprocessors
2. `PerformanceBehaviour<,>`
3. `FusionCacheBehaviour<,>`
4. `CacheInvalidationBehaviour<,>`
5. handler
6. exception handlers if an exception escapes

- [ ] **Step 6: Run focused integration tests**

Run: `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~MediatorCompatibilityTests" -v minimal`
Expected: PASS for validation and exception-path coverage.

- [ ] **Step 7: Commit**

```bash
git add src/Infrastructure/Services/MediatorCompatibility/CompatibilityRequestPipeline.cs src/Application/DependencyInjection.cs src/Application/Common/PublishStrategies/ParallelNoWaitPublisher.cs tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs
git commit -m "feat: bridge mediator pipeline behaviors"
```

## Task 5: Reconnect Scoped Mediator, Notifications, and Existing Consumers

**Files:**
- Create: `src/Infrastructure/Services/MediatorCompatibility/CompatibilityNotificationPublisher.cs`
- Modify: `src/Infrastructure/Services/MediatorWrapper/ScopedMediator.cs`
- Modify: `src/Infrastructure/DependencyInjection.cs`
- Modify: `src/Server.UI/_Imports.razor`
- Modify: `src/Server.UI/Services/DialogServiceHelper.cs`
- Modify: `tests/Application.IntegrationTests/Testing.cs`

- [ ] **Step 1: Write the failing notification and scoped-send tests**

Extend `tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs` to cover:

- `IScopedMediator.Send(...)`
- `IScopedMediator.Publish(...)`

Use a lightweight notification handler with an observable side effect if one can be registered test-only. If not, assert at minimum that `Publish(...)` completes without container or dispatch errors.

- [ ] **Step 2: Run the tests to confirm they fail**

Run: `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~MediatorCompatibilityTests" -v minimal`
Expected: FAIL because notification publishing and scoped dispatch are not fully wired through the compatibility runtime yet.

- [ ] **Step 3: Implement the compatibility notification publisher**

```csharp
internal sealed class CompatibilityNotificationPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationPublisher _publisher;

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
        var executors = handlers.Select(handler =>
            new NotificationHandlerExecutor((message, ct) => handler.Handle((TNotification)message, ct)));

        await _publisher.Publish(executors, notification, cancellationToken);
    }
}
```

- [ ] **Step 4: Update `ScopedMediator` to resolve the compatibility facade**

Keep the current scope-creation pattern, but replace:

```csharp
IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
```

with the repo-owned compatibility `IMediator` service so callers do not change.

- [ ] **Step 5: Keep UI and tests source-compatible**

Do the smallest edits necessary so these files compile against the repo-owned compatibility contracts:

- `src/Server.UI/_Imports.razor`
- `src/Server.UI/Services/DialogServiceHelper.cs`
- `tests/Application.IntegrationTests/Testing.cs`

Preserve existing method signatures and injection shapes.

- [ ] **Step 6: Run focused tests**

Run:

- `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --filter "FullyQualifiedName~ScopedMediatorTests" -v minimal`
- `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~MediatorCompatibilityTests" -v minimal`

Expected: PASS for scoped mediator and notification smoke coverage.

- [ ] **Step 7: Commit**

```bash
git add src/Infrastructure/Services/MediatorCompatibility/CompatibilityNotificationPublisher.cs src/Infrastructure/Services/MediatorWrapper/ScopedMediator.cs src/Infrastructure/DependencyInjection.cs src/Server.UI/_Imports.razor src/Server.UI/Services/DialogServiceHelper.cs tests/Application.IntegrationTests/Testing.cs tests/Application.IntegrationTests/Common/MediatorCompatibility/MediatorCompatibilityTests.cs
git commit -m "feat: reconnect scoped mediator and notifications"
```

## Task 6: Remove Leftover MediatR References and Update Documentation

**Files:**
- Modify: `README.md`
- Modify: `src/Server.UI/Pages/Public/Index.razor`
- Modify: `src/Server.UI/Pages/AI/Chatbot.razor`
- Modify: any remaining `.csproj` files that still reference `MediatR`

- [ ] **Step 1: Search for remaining MediatR references**

Run: `rg -n MediatR src tests README.md`
Expected: only repo-owned compatibility namespace references remain, not package dependency or outdated docs.

- [ ] **Step 2: Update the docs and UI strings**

Use direct text replacements like:

```md
| **Backend** | .NET 10, ASP.NET Core, Mediator, FluentValidation |
```

and update the workflow guidance so it refers to mediator pipeline behavior generically or to the compatibility layer where needed.

- [ ] **Step 3: Run the search again**

Run: `rg -n MediatR src tests README.md`
Expected: no stale user-facing claims that the app is powered by the external MediatR package.

- [ ] **Step 4: Commit**

```bash
git add README.md src/Server.UI/Pages/Public/Index.razor src/Server.UI/Pages/AI/Chatbot.razor
git commit -m "docs: update mediator migration references"
```

## Task 7: Full Verification And Cleanup

**Files:**
- Modify: any files required to fix final compile or test failures uncovered during verification

- [ ] **Step 1: Build the solution**

Run: `dotnet build CleanArchitecture.Blazor.slnx -v minimal`
Expected: BUILD SUCCEEDED

- [ ] **Step 2: Run unit tests**

Run: `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj -v minimal`
Expected: PASS

- [ ] **Step 3: Run integration tests**

Run: `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj -v minimal`
Expected: PASS

- [ ] **Step 4: Re-run the MediatR text search**

Run: `rg -n MediatR src tests README.md`
Expected: only intentionally retained namespace compatibility references remain.

- [ ] **Step 5: Inspect git diff before final handoff**

Run: `git status --short && git diff --stat`
Expected: only mediator-migration-related files are changed.

- [ ] **Step 6: Final commit**

```bash
git add src tests README.md
git commit -m "feat: migrate mediatr runtime to mediator"
```

## Notes For The Implementer

- Keep the compatibility contracts in the `MediatR` and `MediatR.Pipeline` namespaces to avoid touching hundreds of existing source files.
- Do not widen scope into command/query abstraction redesign.
- Prove the custom notification publishing strategy early. It is the highest-risk behavioral difference.
- Preserve the current `IScopedMediator` scope-lifetime semantics.
- If `Mediator` requires request or handler shapes that do not align exactly with the compatibility contracts, isolate that mismatch inside `src/Infrastructure/Services/MediatorCompatibility/*` rather than editing feature code.
