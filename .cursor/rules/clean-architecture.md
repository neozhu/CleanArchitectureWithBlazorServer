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

#### **Query Handler Pattern**
```csharp
// Query in Application/Features/YourFeature/Queries/
public record GetYourEntityQuery(int Id) : IRequest<Result<YourEntityDto>>;

internal sealed class GetYourEntityQueryHandler : IRequestHandler<GetYourEntityQuery, Result<YourEntityDto>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    
    public GetYourEntityQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }
    
    public async Task<Result<YourEntityDto>> Handle(GetYourEntityQuery request, CancellationToken ct)
    {
        await using var db = await _dbContextFactory.CreateAsync(ct);
        var entity = await db.YourEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            
        if (entity == null)
            return Result<YourEntityDto>.Failure("Entity not found");
            
        var dto = _mapper.Map<YourEntityDto>(entity);
        return Result<YourEntityDto>.Success(dto);
    }
}
```

#### **Command Handler Pattern**
```csharp
// Command in Application/Features/YourFeature/Commands/
public class AddEditYourEntityCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CacheKey => YourEntityCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => YourEntityCacheKey.Tags;
}

public class AddEditYourEntityCommandHandler : IRequestHandler<AddEditYourEntityCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public AddEditYourEntityCommandHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddEditYourEntityCommand request, CancellationToken ct)
    {
        await using var db = await _dbContextFactory.CreateAsync(ct);
        
        if (request.Id > 0)
        {
            var item = await db.YourEntities.SingleOrDefaultAsync(x => x.Id == request.Id, ct);
            if (item == null) return await Result<int>.FailureAsync($"Entity with id: [{request.Id}] not found.");
            
            item = _mapper.Map(request, item);
            await db.SaveChangesAsync(ct);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<YourEntity>(request);
            db.YourEntities.Add(item);
            await db.SaveChangesAsync(ct);
            return await Result<int>.SuccessAsync(item.Id);
        }
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

### **4. Database Context Factory Pattern**
```csharp
// ✅ CORRECT - Use IApplicationDbContextFactory in Handlers
public class YourEntityHandler : IRequestHandler<YourRequest, Result<YourResponse>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public YourEntityHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<YourResponse>> Handle(YourRequest request, CancellationToken ct)
    {
        // Always use 'await using' pattern for proper disposal
        await using var db = await _dbContextFactory.CreateAsync(ct);
        
        // Your database operations here
        var entity = await db.YourEntities.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        
        // Don't forget to save changes for write operations
        if (isWriteOperation)
        {
            await db.SaveChangesAsync(ct);
        }
        
        return Result<YourResponse>.Success(response);
    }
}

// ❌ WRONG - Direct IApplicationDbContext injection
public class WrongHandler : IRequestHandler<YourRequest, Result<YourResponse>>
{
    private readonly IApplicationDbContext _context; // Don't do this anymore
    
    public WrongHandler(IApplicationDbContext context) // Old pattern
    {
        _context = context;
    }
}

// ❌ WRONG - DbContextFactory in UI layer
@page "/wrong-pattern"
@inject IApplicationDbContextFactory DbFactory // Don't inject this in UI

@code {
    protected override async Task OnInitializedAsync()
    {
        await using var db = await DbFactory.CreateAsync(); // Wrong! Use Mediator instead
        var data = await db.YourEntities.ToListAsync();
    }
}
```

## **Validation Rules**

### **Architecture Validation Checklist**
- [ ] No `using Infrastructure` statements in UI layer (except Program.cs)
- [ ] No `using Server.UI` statements in Application/Infrastructure layers
- [ ] All services have interfaces in Application layer
- [ ] All data access goes through Mediator (no direct DbContext in UI)
- [ ] Use `IApplicationDbContextFactory` instead of direct `IApplicationDbContext` injection in handlers
- [ ] Always use `await using var db = await _dbContextFactory.CreateAsync(ct);` pattern
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
// ❌ WRONG - Direct DbContext injection
@inject ApplicationDbContext Context
var data = await Context.YourEntities.ToListAsync();

// ❌ WRONG - DbContextFactory injection in UI
@inject IApplicationDbContextFactory DbContextFactory
await using var db = await DbContextFactory.CreateAsync();
var data = await db.YourEntities.ToListAsync();

// ✅ CORRECT - Use Mediator pattern
// 1. Create query in Application layer
public record GetYourEntitiesQuery : IRequest<Result<List<YourEntityDto>>>;
public class GetYourEntitiesQueryHandler : IRequestHandler<GetYourEntitiesQuery, Result<List<YourEntityDto>>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    
    public GetYourEntitiesQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }
    
    public async Task<Result<List<YourEntityDto>>> Handle(GetYourEntitiesQuery request, CancellationToken ct)
    {
        await using var db = await _dbContextFactory.CreateAsync(ct);
        var entities = await db.YourEntities
            .AsNoTracking()
            .ProjectTo<YourEntityDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
        return Result<List<YourEntityDto>>.Success(entities);
    }
}

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
- Always use `await using var db = await _dbContextFactory.CreateAsync(ct);` pattern for proper disposal
- Use `AsNoTracking()` for read-only queries
- Use `ProjectTo<TDto>()` for efficiency
- Include only needed data with `Select()`
- Use proper indexing strategies
- Pass `CancellationToken` to all async database operations
- Avoid multiple DbContext instances in single handler - create once per handler execution

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