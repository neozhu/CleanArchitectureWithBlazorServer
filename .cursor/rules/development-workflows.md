---
description: Development workflows and common patterns for efficient coding
globs: src/**/*.cs, src/**/*.razor
alwaysApply: true
---

# Development Workflows & Common Patterns

## **Feature Development Workflow**

### **1. New Feature Implementation Steps**
1. **Planning Phase**
   - Define feature requirements and scope
   - Identify required permissions in `Application.Common.Security`
   - Plan data model changes in Domain layer
   - Design API contracts (DTOs, Commands, Queries)

2. **Domain Layer Changes**
   - Create or modify entities in `Domain/Entities/`
   - Add value objects if needed in `Domain/ValueObjects/`
   - Define domain events in `Domain/Events/`

3. **Application Layer Implementation**
   - Create DTOs in `Application/Features/{Feature}/DTOs/`
   - Implement Commands in `Application/Features/{Feature}/Commands/`
   - Implement Queries in `Application/Features/{Feature}/Queries/`
   - Add validators in each Command/Query folder
   - Create mapping profiles for AutoMapper
   - Define permissions in `Application/Common/Security/Permissions/`

4. **Infrastructure Layer Updates**
   - Configure EF Core mappings in `Infrastructure/Persistence/Configurations/`
   - Add database migrations
   - Implement any external service integrations

5. **UI Layer Development**
   - Create pages in `Server.UI/Pages/{Feature}/`
   - Create components in `Server.UI/Pages/{Feature}/Components/`
   - Add localization resources
   - Implement proper authorization

### **2. Code Generation Helpers**

#### **Quick CQRS Command Generation**
```csharp
// Template for Commands
public record Create{Entity}Command(
    string Property1,
    int Property2
) : ICacheInvalidatorRequest<Result<int>>;

internal sealed class Create{Entity}CommandHandler : IRequestHandler<Create{Entity}Command, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<Create{Entity}CommandHandler> _logger;

    public Create{Entity}CommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<Create{Entity}CommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(Create{Entity}Command request, CancellationToken ct)
    {
        try
        {
            var entity = new {Entity}(request.Property1, request.Property2);
            _context.{Entities}.Add(entity);
            await _context.SaveChangesAsync(ct);
            
            _logger.LogInformation("{Entity} created with ID: {Id}", nameof({Entity}), entity.Id);
            return Result<int>.Success(entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {Entity}", nameof({Entity}));
            return Result<int>.Failure("Failed to create {Entity}");
        }
    }
}

public sealed class Create{Entity}CommandValidator : AbstractValidator<Create{Entity}Command>
{
    public Create{Entity}CommandValidator()
    {
        RuleFor(x => x.Property1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Property2).GreaterThan(0);
    }
}
```

#### **Quick Query Generation**
```csharp
// Template for Queries
public record Get{Entity}ByIdQuery(int Id) : ICacheableRequest<Result<{Entity}Dto>>
{
    public string CacheKey => $"{Entity}:{Id}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(30);
}

internal sealed class Get{Entity}ByIdQueryHandler : IRequestHandler<Get{Entity}ByIdQuery, Result<{Entity}Dto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public Get{Entity}ByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<{Entity}Dto>> Handle(Get{Entity}ByIdQuery request, CancellationToken ct)
    {
        var entity = await _context.{Entities}
            .AsNoTracking()
            .ProjectTo<{Entity}Dto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        return entity != null
            ? Result<{Entity}Dto>.Success(entity)
            : Result<{Entity}Dto>.Failure($"{Entity} not found");
    }
}
```

#### **Quick Blazor Page Generation**
```razor
@page "/pages/{feature-name}"
@attribute [Authorize(Policy = Permissions.{Feature}.View)]
@using CleanArchitecture.Blazor.Application.Features.{Feature}.DTOs
@using CleanArchitecture.Blazor.Application.Features.{Feature}.Queries.Pagination
@using CleanArchitecture.Blazor.Application.Features.{Feature}.Commands.AddEdit
@using CleanArchitecture.Blazor.Application.Features.{Feature}.Commands.Delete

@inject IStringLocalizer<{Feature}> L

<PageTitle>@Title</PageTitle>

<MudDataGrid ServerData="@(ServerReload)"
             FixedHeader="true"
             FixedFooter="false"
             @bind-RowsPerPage="_defaultPageSize"
             Loading="@_loading"
             MultiSelection="true"
             @bind-SelectedItems="_selectedItems"
             Hover="true" @ref="_dataGrid">
    <ToolBarContent>
        <!-- Toolbar implementation -->
    </ToolBarContent>
    <Columns>
        <!-- Column definitions -->
    </Columns>
    <NoRecordsContent>
        <MudText>@ConstantString.NoRecords</MudText>
    </NoRecordsContent>
    <LoadingContent>
        <MudText>@ConstantString.Loading</MudText>
    </LoadingContent>
    <PagerContent>
        <MudDataGridPager PageSizeOptions="@(new[] { 10, 15, 30, 50, 100 })" />
    </PagerContent>
</MudDataGrid>

@code {
    private string Title => L["{Feature}"];
    private bool _loading;
    private int _defaultPageSize = 15;
    private HashSet<{Entity}Dto> _selectedItems = new();
    private MudDataGrid<{Entity}Dto> _dataGrid = default!;
    private {Feature}sWithPaginationQuery _query = new();

    private async Task<GridData<{Entity}Dto>> ServerReload(GridState<{Entity}Dto> state)
    {
        try
        {
            _loading = true;
            _query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            _query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? false
                                      ? SortDirection.Descending.ToString()
                                      : SortDirection.Ascending.ToString();
            _query.PageNumber = state.Page + 1;
            _query.PageSize = state.PageSize;
            
            var result = await Mediator.Send(_query);
            return new GridData<{Entity}Dto> 
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

## **Common Development Patterns**

### **Error Handling Pattern**
```csharp
// In Handlers
try
{
    // Business logic
    return Result<T>.Success(data);
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error in {Handler}", nameof(Handler));
    return Result<T>.Failure("Database operation failed");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error in {Handler}", nameof(Handler));
    return Result<T>.Failure("An unexpected error occurred");
}

// In UI Components
var result = await Mediator.Send(command);
if (result.Succeeded)
{
    Snackbar.Add(L["Success message"], Severity.Success);
    await RefreshData();
}
else
{
    Snackbar.Add(result.ErrorMessage, Severity.Error);
}
```

### **Permission Check Pattern**
```csharp
// In Pages/Components
[CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;
private {Feature}AccessRights _accessRights = new();

protected override async Task OnInitializedAsync()
{
    _accessRights = await PermissionService.GetAccessRightsAsync<{Feature}AccessRights>();
}

// Conditional UI rendering
@if (_accessRights.Create)
{
    <MudButton StartIcon="@Icons.Material.Outlined.Add" OnClick="OnCreate">
        @ConstantString.New
    </MudButton>
}
```

### **Caching Pattern**
```csharp
// For Queries with caching
public record GetCachedDataQuery() : ICacheableRequest<Result<List<DataDto>>>
{
    public string CacheKey => "cached-data-key";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(15);
}

// For Commands that invalidate cache
[FusionCacheEvict(CacheKeys.GetCacheKey<Entity>())]
public record UpdateEntityCommand(int Id, string Name) : ICacheInvalidatorRequest<Result<int>>;
```

## **Testing Patterns**

### **Unit Test Template**
```csharp
[TestFixture]
public class {Handler}Tests
{
    private Mock<IApplicationDbContext> _mockContext;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<{Handler}>> _mockLogger;
    private {Handler} _handler;

    [SetUp]
    public void SetUp()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<{Handler}>>();
        _handler = new {Handler}(_mockContext.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new {Command/Query} { /* properties */ };
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
    }

    [Test]
    public async Task Handle_InvalidRequest_ReturnsFailure()
    {
        // Arrange & Act & Assert
    }
}
```

## **Performance Optimization Patterns**

### **Database Query Optimization**
```csharp
// Use projection for read-only data
return await _context.Entities
    .AsNoTracking()
    .Where(predicate)
    .ProjectTo<EntityDto>(_mapper.ConfigurationProvider)
    .ToListAsync(ct);

// Use pagination for large datasets
var query = _context.Entities.AsQueryable();
// Apply filters
var totalItems = await query.CountAsync(ct);
var items = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync(ct);
```

### **Component Performance**
```csharp
// Use ShouldRender for expensive components
protected override bool ShouldRender()
{
    return _dataChanged;
}

// Use @key for list items
@foreach (var item in items)
{
    <ComponentItem @key="item.Id" Item="item" />
}
```

## **Debugging and Troubleshooting**

### **Common Issues and Solutions**

1. **Architecture Violation Errors**
   - Check dependency directions
   - Ensure proper interface usage
   - Verify DI registrations

2. **Permission Issues**
   - Check permission definitions
   - Verify user roles and claims
   - Check authorization policies

3. **Database Issues**
   - Check EF Core configurations
   - Verify migration status
   - Check connection strings

4. **Performance Issues**
   - Use AsNoTracking for read operations
   - Implement proper caching
   - Optimize database queries

### **Logging Best Practices**
```csharp
// Structured logging
_logger.LogInformation("Processing {EntityType} with ID {EntityId}", 
    nameof(Entity), entityId);

// Error logging with context
_logger.LogError(ex, "Failed to process {Command} for user {UserId}", 
    nameof(Command), userId);

// Performance logging
using var activity = _logger.BeginScope("Processing {Operation}", operationName);
``` 