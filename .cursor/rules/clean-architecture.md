---
description: Clean Architecture implementation rules for this Blazor Server project
globs: src/**/*.cs, src/**/*.razor
alwaysApply: true
---

# Clean Architecture Rules for CleanArchitectureWithBlazorServer

## **Layer Responsibility Matrix**

| Layer | Allowed Dependencies | Responsibilities | Forbidden |
|-------|---------------------|------------------|-----------|
| **Domain** | None | Entities, ValueObjects, Enums, Events | Any external dependencies |
| **Application** | Domain only | Business logic, DTOs, Interfaces, CQRS | Infrastructure, UI references |
| **Infrastructure** | Application, Domain | Data access, External services, Configuration | UI references |
| **Server.UI** | Application, Domain | Presentation, Components, DI setup | Infrastructure services (except DI) |

## **Mandatory Patterns**

### **1. Service Implementation Pattern**
```csharp
// Step 1: Always create interface in Application layer
namespace CleanArchitecture.Blazor.Application.Common.Interfaces;
public interface IYourService
{
    Task<Result<T>> MethodAsync(RequestDto request, CancellationToken ct = default);
}

// Step 2: Implement in Infrastructure layer  
namespace CleanArchitecture.Blazor.Infrastructure.Services;
public class YourService : IYourService
{
    // Implementation
}

// Step 3: Register in Infrastructure.DependencyInjection
services.AddScoped<IYourService, YourService>();
```

### **2. CQRS Implementation Pattern**
```csharp
// Query in Application/Features/YourFeature/Queries/
public record GetYourEntityQuery(int Id) : IRequest<Result<YourEntityDto>>;

internal sealed class GetYourEntityQueryHandler : IRequestHandler<GetYourEntityQuery, Result<YourEntityDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public GetYourEntityQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<Result<YourEntityDto>> Handle(GetYourEntityQuery request, CancellationToken ct)
    {
        var entity = await _context.YourEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            
        if (entity == null)
            return Result<YourEntityDto>.Failure("Entity not found");
            
        var dto = _mapper.Map<YourEntityDto>(entity);
        return Result<YourEntityDto>.Success(dto);
    }
}
```

### **3. UI Component Pattern**
```csharp
@page "/your-feature"
@attribute [Authorize(Policy = Permissions.YourFeature.View)]
@inject IStringLocalizer<YourComponent> L
@inject IMediator Mediator
@inject ISnackbar Snackbar

<PageTitle>@Title</PageTitle>

<MudDataGrid ServerData="ServerReload" 
             FixedHeader="true"
             Loading="@_loading"
             T="YourEntityDto">
    <!-- Grid implementation -->
</MudDataGrid>

@code {
    private bool _loading;
    private string Title => L["Your Feature"];
    
    private async Task<GridData<YourEntityDto>> ServerReload(GridState<YourEntityDto> state)
    {
        try
        {
            _loading = true;
            var query = new GetYourEntitiesQuery();
            var result = await Mediator.Send(query);
            
            return new GridData<YourEntityDto> 
            { 
                TotalItems = result.TotalItems, 
                Items = result.Items 
            };
        }
        finally
        {
            _loading = false;
        }
    }
}
```

## **Validation Rules**

### **Architecture Validation Checklist**
- [ ] No `using Infrastructure` statements in UI layer (except Program.cs)
- [ ] No `using Server.UI` statements in Application/Infrastructure layers
- [ ] All services have interfaces in Application layer
- [ ] All data access goes through Mediator (no direct DbContext in UI)
- [ ] All configurations use IOptions pattern
- [ ] All constants are in Application.Common.Constants
- [ ] All permissions are in Application.Common.Security

### **Code Quality Checklist**
- [ ] Async methods end with 'Async'
- [ ] CancellationToken parameter included
- [ ] Result pattern used for operations that can fail  
- [ ] FluentValidation for input validation
- [ ] XML documentation for public APIs
- [ ] Proper error handling with logging

## **Common Fixes for Architecture Violations**

### **Fix: UI layer accessing Infrastructure**
```csharp
// ❌ WRONG
@inject SomeInfrastructureService Service

// ✅ CORRECT  
// 1. Create interface in Application layer
public interface ISomeService { }
// 2. Move implementation to Infrastructure
public class SomeService : ISomeService { }
// 3. Register in Infrastructure DI
services.AddScoped<ISomeService, SomeService>();
// 4. Inject interface in UI
@inject ISomeService Service
```

### **Fix: Direct database access in UI**
```csharp
// ❌ WRONG
@inject ApplicationDbContext Context
var data = await Context.YourEntities.ToListAsync();

// ✅ CORRECT
// 1. Create query in Application layer
public record GetYourEntitiesQuery : IRequest<Result<List<YourEntityDto>>>;
// 2. Inject Mediator in UI
@inject IMediator Mediator
var result = await Mediator.Send(new GetYourEntitiesQuery());
```

### **Fix: Configuration access violations**
```csharp
// ❌ WRONG
@inject IConfiguration Config
var value = Config["Section:Key"];

// ✅ CORRECT
// 1. Create settings interface in Application
public interface IYourSettings { string Property { get; } }
// 2. Create implementation in Infrastructure  
public class YourSettings : IYourSettings { }
// 3. Register with IOptions
services.Configure<YourSettings>(config.GetSection("Section"));
// 4. Inject interface in UI
@inject IYourSettings Settings
```

## **Performance Guidelines**

### **Database Queries**
- Use `AsNoTracking()` for read-only queries
- Use `ProjectTo<TDto>()` for efficiency
- Include only needed data with `Select()`
- Use proper indexing strategies

### **Caching Strategy**
- Apply `[FusionCacheEvict]` attributes on commands that modify data
- Use cache keys from `GlobalCacheKeys` class
- Set appropriate cache durations

### **Component Performance**
- Use `@key` directive for list items
- Minimize re-renders with proper state management
- Use `ShouldRender()` override when necessary

## **Security Best Practices**

### **Authorization Flow**
1. Page-level: `@attribute [Authorize(Policy = Permissions.Feature.Action)]`
2. Method-level: `await PermissionService.HasPermissionAsync(permission)`
3. UI conditional: `@if (hasPermission) { }`

### **Input Validation**
1. Client-side: MudBlazor validation
2. Command/Query: FluentValidation validators
3. Domain-level: Entity validation rules

### **Data Protection**
- Use HTTPS for all communications
- Implement proper CORS policies  
- Validate all inputs at multiple layers
- Use parameterized queries (EF Core handles this) 