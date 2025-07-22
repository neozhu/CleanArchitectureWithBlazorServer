### Summary
This pull request merges the changes from the `ApplicationDbContextFactory` branch into `main`.

#### Commits included:

1. **Enhance application architecture by introducing IAsyncDisposable to IApplicationDbContext and refactoring services to use IApplicationDbContextFactory**
   - Updated `IApplicationDbContext` to inherit from `IAsyncDisposable` for better resource management.
   - Refactored `IPicklistService` and `ITenantService` to use asynchronous methods for initialization and refresh operations.
   - Modified various command and query handlers to utilize `IApplicationDbContextFactory` for database context creation, ensuring adherence to Clean Architecture principles.
   - Improved error handling and performance in data operations across multiple features.
   - These changes contribute to a more robust and maintainable architecture while aligning with the established coding standards.

2. **Refactor IRoleService and IUserService to use asynchronous methods for initialization and refresh**
   - Updated `IRoleService` and `IUserService` interfaces to replace synchronous methods with asynchronous counterparts (`InitializeAsync` and `RefreshAsync`).
   - Refactored `RoleService` and `UserService` implementations to support asynchronous operations, improving performance and responsiveness.
   - Modified various components and pages to utilize the new asynchronous methods, ensuring compliance with Clean Architecture principles.
   - These changes enhance the overall architecture and maintainability of the application while adhering to established coding standards.

3. **Remove IScopedMediator and simplify mediator usage**
   - Removed the `IScopedMediator` interface and its implementation, replacing it with direct usage of the `IMediator` interface throughout the application.
   - Key changes include:
     - Removal of `IScopedMediator` from codebase and its registration in DI.
     - Updates to `DispatchDomainEventsInterceptor.cs` to use `IMediator`.
     - Complete removal of the `ScopedMediator` class.
     - Simplification of error display in `ErrorPageComponent.razor` and `Error.razor`.
     - Updates to `_Imports.razor` and `Testing.cs` to align with the new mediator usage.
   - These changes streamline the architecture and reduce complexity related to mediator services.

4. **Update app version and FluentAssertions package**
   - Updated application version from 1.2.43 to 1.2.45 in `appsettings.json`.
   - Upgraded `FluentAssertions` package from 8.4.0 to 8.5.0 in:
     - `Application.IntegrationTests.csproj`
     - `Application.UnitTests.csproj`
     - `Domain.UnitTests.csproj`

Please review and merge if everything looks good.