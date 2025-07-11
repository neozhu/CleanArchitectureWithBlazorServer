# GitHub Copilot Instructions

> **Purpose:** Comprehensive development guidelines ensuring high-quality, secure, and maintainable code following Clean Architecture principles.

---

## 1  Project Overview

| Setting        | Value                                                                                |
| -------------- | ------------------------------------------------------------------------------------ |
| **Solution**   | `CleanArchitectureWithBlazorServer.sln`                                              |
| **Runtime**    | .NET 9 · Blazor Server                                                               |
| **UI Library** | MudBlazor (latest stable)                                                            |
| **Patterns**   | Clean Architecture · CQRS · MediatR                                                  |
| **Tooling**    | AutoMapper · FluentValidation · EF Core · Identity.EntityFrameworkCore · FusionCache |
| **Testing**    | NUnit · bUnit · FluentAssertions · Moq · Testcontainers                             |
| **Quality**    | SonarQube · CodeQL · Dependabot                                                      |

---

## 2  Architecture Layers & Responsibilities

| Layer / Purpose                                 | Physical Path               | Responsibilities                                                                  | Root Namespace                                           |
| ----------------------------------------------- | --------------------------- | --------------------------------------------------------------------------------- | -------------------------------------------------------- |
| **Domain** (Enterprise core)                    | `src/Domain/`               | Entities, Enums, Domain Events, Business Rules                                    | `CleanArchitectureWithBlazorServer.Domain`               |
| **Application** (Use‑cases)                     | `src/Application/`          | CQRS Commands & Queries, Handlers, DTOs, Validators, Mapping Profiles, Interfaces | `CleanArchitectureWithBlazorServer.Application`          |
| **Infrastructure** (Framework & external calls) | `src/Infrastructure/`       | EF Core persistence, Identity, External APIs, Background Jobs, Caching           | `CleanArchitectureWithBlazorServer.Infrastructure`       |
| **Migrators** (DB migrations)                   | `src/Migrators/<Provider>/` | FluentMigrator projects for MSSQL, PostgreSQL, SQLite                             | `CleanArchitectureWithBlazorServer.Migrators.<Provider>` |
| **UI** (Blazor Server)                          | `src/Server.UI/`            | `.razor` pages, components, `Program.cs`, DI wiring, Client-side logic           | `CleanArchitectureWithBlazorServer.Server.UI`            |
| **Tests**                                       | `tests/`                    | Unit / Integration / E2E tests                                                   | Same as tested assembly                                  |

### Layer Dependencies (MUST follow)
```
UI → Application → Domain
Infrastructure → Application → Domain
Tests → Any layer (for testing purposes only)
```

---

## 3  Naming Conventions & Patterns

| Artifact            | Pattern                                    | Example                                    |
| ------------------- | ------------------------------------------ | ------------------------------------------ |
| **Command**         | `{Verb}{Entity}Command`                   | `CreateProductCommand`                     |
| **Query**           | `Get{Entity}[By{Criteria}]Query`          | `GetProductByIdQuery`                      |
| **Handler**         | `{Command/Query}Handler`                  | `CreateProductCommandHandler`              |
| **DTO**             | `{Entity}Dto`                             | `ProductDto`                               |
| **Mapping Profile** | `{Entity}Profile`                         | `ProductProfile`                           |
| **Validator**       | `{Command/Query}Validator`                | `CreateProductCommandValidator`            |
| **Entity**          | `PascalCase` (singular)                   | `Product`                                  |
| **Service**         | `I{Service}Service` / `{Service}Service`  | `IEmailService` / `EmailService`           |
| **Component**       | `PascalCase.razor`                        | `ProductList.razor`                        |
| **Test Class**      | `{ClassUnderTest}Tests`                   | `CreateProductCommandHandlerTests`        |

---

## 4  Code Generation Standards

### 4.1 Mandatory Requirements

1. **Dependency Injection**: Always use constructor injection with `readonly` fields
2. **Async Programming**: Use `async/await` consistently, never block with `.Result` or `.Wait()`
3. **Null Safety**: Use `ArgumentNullException.ThrowIfNull()` and nullable reference types
4. **Validation**: Provide FluentValidation validators for all Commands/Queries
5. **Error Handling**: Use Result pattern or custom exceptions with proper error codes
6. **Logging**: Add structured logging with appropriate log levels
7. **Caching**: Use FusionCache for read-heavy operations
8. **Security**: Implement proper authorization and input validation
9. **Testing**: Write comprehensive unit and integration tests
10. **Documentation**: Add XML documentation for public APIs

### 4.2 Performance Guidelines

```csharp
// ✅ Good: Async with ConfigureAwait(false) in libraries
public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken ct)
{
    var product = await _context.Products
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == request.Id, ct)
        .ConfigureAwait(false);
    return await Result<ProductDto>.SuccessAsync(_mapper.Map<ProductDto>(product));
}

// ✅ Good: Use AsNoTracking for read-only queries
public async Task<List<Product>> GetProductsAsync(CancellationToken ct)
{
    return await _context.Products
        .AsNoTracking()
        .ToListAsync(ct);
}

// ✅ Good: Batch operations instead of loops
public async Task UpdateProductsAsync(List<Product> products, CancellationToken ct)
{
    _context.Products.UpdateRange(products);
    await _context.SaveChangesAsync(ct);
}
```

### 4.3 Security Best Practices

```csharp
// ✅ Input Validation
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .Matches(@"^[a-zA-Z0-9\s\-_]+$"); // Prevent injection
            
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThan(1000000);
    }
}

// ✅ Authorization at Component level
@attribute [Authorize(Policy = Permissions.Products.Create)]

@code {
    private async Task OnCreateProduct()
    {
        var command = new CreateProductCommand(ProductName, Price, Description);
        var result = await Mediator.Send(command);
        if (result.Succeeded)
        {
            Snackbar.Add("Product created successfully", Severity.Success);
            await LoadProducts();
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
    }
}
```

---

## 5  CQRS Implementation Templates

### 5.1 Command Pattern
```csharp
// Command
public sealed record CreateProductCommand(
    string Name, 
    decimal Price, 
    string Description) : ICacheInvalidatorRequest<Result<int>>;

// Handler
internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IApplicationDbContext context, 
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        try
        {
            var entity = new Product(request.Name, request.Price, request.Description);
            
            _context.Products.Add(entity);
            await _context.SaveChangesAsync(ct);
            
            _logger.LogInformation("Product created successfully with ID: {ProductId}", entity.Id);
            
            return await Result<int>.SuccessAsync(entity.Id, "Product created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            return await Result<int>.FailureAsync("Failed to create product");
        }
    }
}

// Validator
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");
            
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
            
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}
```

### 5.2 Query Pattern with Caching
```csharp
// Query
public sealed record GetProductByIdQuery(int Id) : ICacheableRequest<Result<ProductDto>>
{
    public string CacheKey => $"Product:{Id}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(30);
}

// Handler
internal sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFusionCache _cache;

    public GetProductByIdQueryHandler(
        IApplicationDbContext context, 
        IMapper mapper,
        IFusionCache cache)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _cache.GetOrSetAsync(
            request.CacheKey,
            async _ => await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, ct),
            request.Expiry,
            token: ct);

        if (product == null)
            return await Result<ProductDto>.FailureAsync("Product not found");

        var dto = _mapper.Map<ProductDto>(product);
        return await Result<ProductDto>.SuccessAsync(dto);
    }
}
```

---

## 6  Blazor Component Best Practices

### 6.1 Component Structure
```razor
@page "/pages/products"
@using CleanArchitectureWithBlazorServer.Application.Features.Products.DTOs
@using CleanArchitectureWithBlazorServer.Application.Features.Products.Queries.Export
@using CleanArchitectureWithBlazorServer.Application.Features.Products.Queries.Pagination
@using CleanArchitectureWithBlazorServer.Application.Features.Products.Commands.AddEdit
@using CleanArchitectureWithBlazorServer.Application.Features.Products.Commands.Delete
@using CleanArchitectureWithBlazorServer.Application.Features.Products.Commands.Import
@using CleanArchitectureWithBlazorServer.Domain.Common.Enums

@attribute [Authorize(Policy = Permissions.Products.View)]
@inject IStringLocalizer<Products> L
@inject BlazorDownloadFileService BlazorDownloadFileService

<PageTitle>@Title</PageTitle>

<MudDataGrid ServerData="@(ServerReload)"
             FixedHeader="true"
             FixedFooter="false"
             @bind-RowsPerPage="_defaultPageSize"
             Loading="@_loading"
             MultiSelection="true"
             @bind-SelectedItems="_selectedItems"
             ColumnResizeMode="ResizeMode.Column"
             Hover="true" @ref="_productsGrid">
    <ToolBarContent>
        <MudStack Row Spacing="0" Class="flex-grow-1" Justify="Justify.SpaceBetween">
            <MudStack Row AlignItems="AlignItems.Start">
                <MudIcon Icon="@Icons.Material.Filled.QrCodeScanner" Size="Size.Large" />
                <MudStack Spacing="0">
                    <MudText Typo="Typo.subtitle2" Class="mb-2">@L[_currentDto.GetClassDescription()]</MudText>
                    <MudEnumSelect Style="min-width:120px"
                                   TEnum="ProductListView"
                                   ValueChanged="OnListViewChanged"
                                   Value="_productsQuery.ListView"
                                   Dense="true"
                                   Label="@L["List View"]">
                    </MudEnumSelect>
                </MudStack>
            </MudStack>
            <MudStack Spacing="0" AlignItems="AlignItems.End">
                <MudToolBar Dense WrapContent="true" Class="py-1 px-0">
                    <MudButton Disabled="@_loading"
                               OnClick="@(() => OnRefresh())"
                               StartIcon="@Icons.Material.Outlined.Refresh">
                        @ConstantString.Refresh
                    </MudButton>
                    @if (_accessRights.Create)
                    {
                        <MudButton StartIcon="@Icons.Material.Outlined.Add"
                                   OnClick="OnCreateProduct">
                            @ConstantString.New
                        </MudButton>
                    }
                    <MudMenu TransformOrigin="Origin.BottomRight"
                             AnchorOrigin="Origin.BottomRight"
                             EndIcon="@Icons.Material.Filled.MoreVert"
                             Label="@ConstantString.More">
                        @if (_accessRights.Create)
                        {
                            <MudMenuItem Disabled="@(_selectedItems.Count != 1)" OnClick="OnCloneProduct">
                                @ConstantString.Clone
                            </MudMenuItem>
                        }
                        @if (_accessRights.Delete)
                        {
                            <MudMenuItem Disabled="@(!(_selectedItems.Count > 0))"
                                         OnClick="OnDeleteSelectedProducts">
                                @ConstantString.Delete
                            </MudMenuItem>
                        }
                        @if (_accessRights.Export)
                        {
                            <MudMenuItem Disabled="@_exporting" OnClick="OnExport">
                                @ConstantString.Export
                            </MudMenuItem>
                            <MudMenuItem Disabled="@_pdfExporting" OnClick="OnExportPDF">
                                @ConstantString.ExportPDF
                            </MudMenuItem>
                        }
                        @if (_accessRights.Import)
                        {
                            <MudMenuItem>
                                <MudFileUpload T="IBrowserFile"
                                               FilesChanged="OnFileImport"
                                               Accept=".xlsx">
                                    <ActivatorContent>
                                        <MudButton Class="pa-0 ma-0"
                                                   Style="font-weight:400;text-transform:none;"
                                                   Variant="Variant.Text"
                                                   Disabled="@_uploading">
                                            @ConstantString.Import
                                        </MudButton>
                                    </ActivatorContent>
                                </MudFileUpload>
                            </MudMenuItem>
                        }
                    </MudMenu>
                </MudToolBar>
                <MudStack Row Spacing="1">
                    @if (_accessRights.Search)
                    {
                        <MudTextField T="string"
                                      ValueChanged="@(s => OnSearch(s))"
                                      Value="@_productsQuery.Keyword"
                                      Placeholder="@ConstantString.Search"
                                      Adornment="Adornment.End"
                                      AdornmentIcon="@Icons.Material.Filled.Search"
                                      IconSize="Size.Small">
                        </MudTextField>
                    }
                </MudStack>
            </MudStack>
        </MudStack>
    </ToolBarContent>
    <Columns>
        <SelectColumn ShowInFooter="false"></SelectColumn>
        <TemplateColumn HeaderStyle="width:60px" Title="@ConstantString.Actions" Sortable="false">
            <CellTemplate>
                @if (_accessRights.Edit || _accessRights.Delete)
                {
                    <MudMenu Icon="@Icons.Material.Filled.Edit"
                             Variant="Variant.Outlined"
                             Size="Size.Small"
                             Dense="true"
                             IconColor="Color.Info"
                             AnchorOrigin="Origin.CenterLeft">
                        @if (_accessRights.Edit)
                        {
                            <MudMenuItem OnClick="@(() => OnEditProduct(context.Item))">
                                @ConstantString.Edit
                            </MudMenuItem>
                        }
                        @if (_accessRights.Delete)
                        {
                            <MudMenuItem OnClick="@(() => OnDeleteProduct(context.Item))">
                                @ConstantString.Delete
                            </MudMenuItem>
                        }
                    </MudMenu>
                }
                else
                {
                    <MudTooltip Text="@ConstantString.NoAllowed" Delay="300">
                        <MudIconButton Variant="Variant.Outlined"
                                       Disabled="true"
                                       Icon="@Icons.Material.Filled.DoNotTouch"
                                       Size="Size.Small"
                                       Color="Color.Surface">
                        </MudIconButton>
                    </MudTooltip>
                }
            </CellTemplate>
        </TemplateColumn>
        <PropertyColumn Property="x => x.Brand" Title="@L[_currentDto.GetMemberDescription(x => x.Brand)]">
            <FooterTemplate>
                @ConstantString.Selected: @_selectedItems.Count
            </FooterTemplate>
        </PropertyColumn>
        <PropertyColumn Property="x => x.Name" Title="@L[_currentDto.GetMemberDescription(x => x.Name)]">
            <CellTemplate>
                <div class="d-flex flex-column">
                    <MudText>@context.Item.Name</MudText>
                    <MudText Typo="Typo.body2" Class="mud-text-secondary">
                        @context.Item.Description
                    </MudText>
                </div>
            </CellTemplate>
        </PropertyColumn>
        <PropertyColumn Property="x => x.Price" Title="@L[_currentDto.GetMemberDescription(x => x.Price)]">
            <FooterTemplate>
                @ConstantString.SelectedTotal: @_selectedItems.Sum(x => x.Price)
            </FooterTemplate>
        </PropertyColumn>
        <PropertyColumn Property="x => x.Unit" Title="@L[_currentDto.GetMemberDescription(x => x.Unit)]" />
    </Columns>
    <NoRecordsContent>
        <MudText>@ConstantString.NoRecords</MudText>
    </NoRecordsContent>
    <LoadingContent>
        <MudText>@ConstantString.Loading</MudText>
    </LoadingContent>
    <PagerContent>
        <MudDataGridPager PageSizeOptions="@(new[] { 10, 15, 30, 50, 100, 500, 1000 })" />
    </PagerContent>
</MudDataGrid>

@code {
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;
    [CascadingParameter] private UserProfile? UserProfile { get; set; }
    
    public string? Title { get; private set; }
    private HashSet<ProductDto> _selectedItems = new();
    private MudDataGrid<ProductDto> _productsGrid = default!;
    private ProductDto _currentDto = new();
    private ProductsAccessRights _accessRights = new();
    private bool _loading;
    private bool _uploading;
    private bool _exporting;
    private bool _pdfExporting;
    private int _defaultPageSize = 15;

    private ProductsWithPaginationQuery _productsQuery { get; } = new();

    protected override async Task OnInitializedAsync()
    {
        Title = L[_currentDto.GetClassDescription()];
        _accessRights = await PermissionService.GetAccessRightsAsync<ProductsAccessRights>();
    }

    private async Task<GridData<ProductDto>> ServerReload(GridState<ProductDto> state)
    {
        try
        {
            _loading = true;
            _productsQuery.CurrentUser = UserProfile;
            _productsQuery.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            _productsQuery.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                                              ? SortDirection.Descending.ToString()
                                              : SortDirection.Ascending.ToString();
            _productsQuery.PageNumber = state.Page + 1;
            _productsQuery.PageSize = state.PageSize;
            var result = await Mediator.Send(_productsQuery).ConfigureAwait(false);
            return new GridData<ProductDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSearch(string text)
    {
        _selectedItems = new HashSet<ProductDto>();
        _productsQuery.Keyword = text;
        await _productsGrid.ReloadServerData();
    }

    private async Task OnListViewChanged(ProductListView listview)
    {
        _productsQuery.ListView = listview;
        await _productsGrid.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        ProductCacheKey.Refresh();
        _selectedItems = new HashSet<ProductDto>();
        _productsQuery.Keyword = string.Empty;
        await _productsGrid.ReloadServerData();
    }

    private async Task OnCreateProduct()
    {
        var command = new AddEditProductCommand { Pictures = new List<ProductImage>() };
        await ShowProductEditFormDialog(string.Format(ConstantString.CreateAnItem, L["Product"]), command);
    }

    private async Task OnEditProduct(ProductDto dto)
    {
        var command = Mapper.Map<AddEditProductCommand>(dto);
        await ShowProductEditFormDialog(string.Format(ConstantString.EditTheItem, L["Product"]), command);
    }

    private async Task OnDeleteProduct(ProductDto dto)
    {
        var contentText = string.Format(ConstantString.DeleteConfirmation, dto.Name);
        var command = new DeleteProductCommand(new[] { dto.Id });
        await DialogServiceHelper.ShowDeleteConfirmationDialogAsync(command, ConstantString.DeleteConfirmationTitle, contentText, async () =>
        {
            await InvokeAsync(async () =>
            {
                await _productsGrid.ReloadServerData();
                _selectedItems.Clear();
            });
        });
    }

    private async Task OnDeleteSelectedProducts()
    {
        var contentText = string.Format(ConstantString.DeleteConfirmWithSelected, _selectedItems.Count);
        var command = new DeleteProductCommand(_selectedItems.Select(x => x.Id).ToArray());
        await DialogServiceHelper.ShowDeleteConfirmationDialogAsync(command, ConstantString.DeleteConfirmationTitle, contentText, async () =>
        {
            await InvokeAsync(async () =>
            {
                await _productsGrid.ReloadServerData();
                _selectedItems.Clear();
            });
        });
    }

    private async Task OnExport()
    {
        _exporting = true;
        var request = new ExportProductsQuery
        {
            Keyword = _productsQuery.Keyword,
            ListView = _productsQuery.ListView,
            OrderBy = _productsGrid.SortDefinitions.Values.FirstOrDefault()?.SortBy ?? "Id",
            SortDirection = _productsGrid.SortDefinitions.Values.FirstOrDefault()?.Descending ?? false
                                ? SortDirection.Descending.ToString()
                                : SortDirection.Ascending.ToString(),
            CurrentUser = UserProfile,
            ExportType = ExportType.Excel
        };
        var result = await Mediator.Send(request);
        if (result.Succeeded)
        {
            await BlazorDownloadFileService.DownloadFileAsync($"{L["Products"]}.xlsx", result.Data!, "application/octet-stream");
            Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
        }
        _exporting = false;
    }

    private async Task OnFileImport(IBrowserFile file)
    {
        _uploading = true;
        var stream = new MemoryStream();
        await file.OpenReadStream(GlobalVariable.MaxAllowedSize).CopyToAsync(stream);
        var command = new ImportProductsCommand(file.Name, stream.ToArray());
        var result = await Mediator.Send(command);
        if (result.Succeeded)
        {
            await _productsGrid.ReloadServerData();
            Snackbar.Add($"{ConstantString.ImportSuccess}", Severity.Info);
        }
        else
        {
            foreach (var msg in result.Errors)
            {
                Snackbar.Add($"{msg}", Severity.Error);
            }
        }
        _uploading = false;
    }

    private async Task ShowProductEditFormDialog(string title, AddEditProductCommand command)
    {
        var parameters = new DialogParameters<ProductFormDialog>
        {
            { x => x.Refresh, () => _productsGrid.ReloadServerData() },
            { x => x._model, command }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<ProductFormDialog>(title, parameters, options);
        var state = await dialog.Result;
        if (state is not null && !state.Canceled)
        {
            await _productsGrid.ReloadServerData();
            _selectedItems.Clear();
        }
    }
}
```

### 6.2 Component Guidelines
- **Size Limit**: Keep components under 300 lines. Split larger components into smaller ones
- **State Management**: Use `@bind` for two-way data binding
- **Error Handling**: Always wrap async operations in try-catch blocks
- **Loading States**: Show loading indicators for async operations
- **Accessibility**: Include proper ARIA labels and keyboard navigation
- **Responsive Design**: Use MudBlazor's grid system for responsive layouts
- **MudBlazor Standards**: Always use standard MudBlazor components without custom CSS styling. Maintain consistency and simplicity by leveraging built-in component properties, themes, and variants instead of adding custom styles

### 6.3 MudBlazor Usage Examples

```razor
<!-- ✅ Good: Keep components simple, use MudGlobal for defaults -->
<MudButton StartIcon="Icons.Material.Filled.Add">
    Add Product
</MudButton>

<MudTextField T="string" 
              Label="Product Name" 
              @bind-Value="ProductName"
              For="@(() => ProductName)" />

<MudDataGrid Items="@products" 
             Filterable="true" 
             SortMode="SortMode.Multiple">
    <Columns>
        <PropertyColumn Property="x => x.Name" Title="Name" />
        <PropertyColumn Property="x => x.Price" Title="Price" Format="C" />
    </Columns>
</MudDataGrid>

<!-- ❌ Bad: Don't add custom styles -->
<MudButton style="background-color: red; padding: 20px;">
    Custom Styled Button
</MudButton>

<div class="my-custom-grid-wrapper">
    <MudDataGrid class="custom-grid-style" Items="@products">
        <!-- Custom CSS defeats MudBlazor's theming -->
    </MudDataGrid>
</div>

<!-- ✅ Good: Use MudBlazor spacing classes when needed -->
<MudButton Color="Color.Error" Class="ma-2">
    Delete
</MudButton>

<MudPaper Class="pa-4 ma-2">
    <MudText Typo="Typo.h6">Product Details</MudText>
    <MudDivider Class="my-2" />
    <MudText>Content goes here</MudText>
</MudPaper>
```

---

## 7  Advanced Caching Strategy

### 7.1 Cache Key Patterns
```csharp
// Entity-based caching
public static class CacheKeys
{
    public static string Product(int id) => $"product:{id}";
    public static string ProductList(string filter = "") => $"products:list:{filter.GetHashCode()}";
    public static string UserProfile(Guid userId) => $"user:profile:{userId}";
    public static string UserPermissions(Guid userId) => $"user:permissions:{userId}";
}

// Cache invalidation
public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly IFusionCache _cache;

    public ProductCreatedEventHandler(IFusionCache cache)
    {
        _cache = cache;
    }

    public async Task Handle(ProductCreatedEvent notification, CancellationToken ct)
    {
        // Invalidate list caches
        await _cache.RemoveByPrefixAsync("products:list:", token: ct);
    }
}
```

### 7.2 Caching Configuration
```csharp
// In Infrastructure/DependencyInjection.cs
services.AddFusionCache()
    .WithDefaultEntryOptions(options =>
    {
        options.Duration = TimeSpan.FromMinutes(5);
        options.JitterMaxDuration = TimeSpan.FromSeconds(30);
        options.FailSafeMaxDuration = TimeSpan.FromHours(1);
        options.FailSafeThrottleDuration = TimeSpan.FromSeconds(10);
    })
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(serviceProvider => 
        serviceProvider.GetRequiredService<IDistributedCache>());
```

---

## 8  Error Handling & Logging

### 8.1 Result Pattern Implementation
```csharp
public class Result<T>
{
    public bool Succeeded { get; init; }
    public T Data { get; init; } = default!;
    public string ErrorMessage { get; init; } = string.Empty;
    public List<string> ErrorMessages { get; init; } = new();
    public int ErrorCode { get; init; }

    public static async Task<Result<T>> SuccessAsync(T data, string message = "")
    {
        return new Result<T>
        {
            Succeeded = true,
            Data = data,
            ErrorMessage = message
        };
    }

    public static async Task<Result<T>> FailureAsync(string errorMessage, int errorCode = 400)
    {
        return new Result<T>
        {
            Succeeded = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }
}
```

### 8.2 Structured Logging
```csharp
internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public CreateProductCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["ProductName"] = request.Name,
            ["UserId"] = _currentUserService.UserId,
            ["OperationId"] = Guid.NewGuid()
        });

        _logger.LogInformation("Creating product {ProductName} for user {UserId}", 
            request.Name, _currentUserService.UserId);

        try
        {
            var entity = new Product(request.Name, request.Price, request.Description);
            
            _context.Products.Add(entity);
            await _context.SaveChangesAsync(ct);
            
            _logger.LogInformation("Product created successfully with ID {ProductId}", entity.Id);
            return await Result<int>.SuccessAsync(entity.Id, "Product created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create product {ProductName}", request.Name);
            return await Result<int>.FailureAsync("Failed to create product");
        }
    }
}
```

---

## 9  Security Implementation

### 9.1 Permission Set Definition
```csharp
// src/Infrastructure/PermissionSet/Products.cs
public static partial class Permissions
{
    [DisplayName("Product Permissions")]
    [Description("Set permissions for product operations")]
    public static class Products
    {
        [Description("Allows viewing product details")]
        public const string View = "Permissions.Products.View";

        [Description("Allows creating new product records")]
        public const string Create = "Permissions.Products.Create";

        [Description("Allows modifying existing product details")]
        public const string Edit = "Permissions.Products.Edit";

        [Description("Allows deleting product records")]
        public const string Delete = "Permissions.Products.Delete";

        [Description("Allows searching for product records")]
        public const string Search = "Permissions.Products.Search";

        [Description("Allows exporting product records")]
        public const string Export = "Permissions.Products.Export";

        [Description("Allows importing product records")]
        public const string Import = "Permissions.Products.Import";
    }
}

// Access Rights Model
public class ProductsAccessRights
{
    public bool View { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Search { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
}
```

### 9.2 Authorization in Components
```csharp
// Page/Component Authorization
@attribute [Authorize(Policy = Permissions.Products.View)]

// Code-behind Permission Checking
@code {
    private ProductsAccessRights _accessRights = new();

    protected override async Task OnInitializedAsync()
    {
        _accessRights = await PermissionService.GetAccessRightsAsync<ProductsAccessRights>();
    }

    // Conditional UI rendering based on permissions
    @if (_accessRights.Create)
    {
        <MudButton StartIcon="@Icons.Material.Outlined.Add"
                   OnClick="OnCreateProduct">
            @ConstantString.New
        </MudButton>
    }
}
```

### 9.3 Authorization Policies (Auto-Generated)
```csharp
// In Infrastructure/DependencyInjection.cs - Authorization policies are automatically created
services.AddAuthorizationCore(options =>
{
    // Auto-generate policies for all permission constants
    foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                 c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
    {
        var propertyValue = prop.GetValue(null);
        if (propertyValue is not null)
            options.AddPolicy((string)propertyValue,
                policy => policy.RequireClaim(ApplicationClaimTypes.Permission, (string)propertyValue));
    }
});
```

### 9.4 Authorization in Handlers
```csharp
// Handler implementation (no [Authorize] attribute needed on handlers)
internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // Authorization is handled at the API/Component level
        // Focus on business logic in handlers
        
        var entity = new Product(request.Name, request.Price);
        _context.Products.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return await Result<int>.SuccessAsync(entity.Id);
    }
}
```

### 9.5 Input Sanitization & Validation
```csharp
// FluentValidation with Security Rules
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .Matches(@"^[a-zA-Z0-9\s\-_]+$") // Prevent XSS/injection
            .WithMessage("Product name contains invalid characters");
            
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .Must(BeValidHtml) // Custom HTML validation
            .WithMessage("Description contains potentially dangerous content");
    }

    private bool BeValidHtml(string html)
    {
        // Use HtmlSanitizer or similar library
        return !html.Contains("<script>", StringComparison.OrdinalIgnoreCase);
    }
}

// Input Sanitization Utilities
public static class SecurityHelper
{
    public static string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Use HtmlSanitizer library for production
        return Regex.Replace(input, @"<[^>]*>", string.Empty);
    }

    public static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }

    public static bool IsValidFileExtension(string fileName, string[] allowedExtensions)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}
```

---

## 10  Testing Standards

### 10.1 Unit Test Template
```csharp
[TestFixture]
public class CreateProductCommandHandlerTests
{
    private Mock<IApplicationDbContext> _mockContext;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<CreateProductCommandHandler>> _mockLogger;
    private CreateProductCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<CreateProductCommandHandler>>();
        _handler = new CreateProductCommandHandler(_mockContext.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateProductCommand("Test Product", 10.99m, "Description");
        var cancellationToken = CancellationToken.None;

        _mockContext.Setup(x => x.SaveChangesAsync(cancellationToken))
                   .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task Handle_DatabaseException_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateProductCommand("Test Product", 10.99m, "Description");
        var cancellationToken = CancellationToken.None;

        _mockContext.Setup(x => x.SaveChangesAsync(cancellationToken))
                   .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Failed to create product");
    }
}
```

### 10.2 Integration Test Base
```csharp
public abstract class IntegrationTestBase : IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly ApplicationDbContext Context;

    protected IntegrationTestBase()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace with test database
                    services.RemoveDbContext<ApplicationDbContext>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

        Client = Factory.CreateClient();
        Context = Factory.Services.GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        Context?.Dispose();
        Client?.Dispose();
        Factory?.Dispose();
    }
}
```

---

## 11  Performance Optimization

### 11.1 Database Optimization
```csharp
// ✅ Use projection for read-only scenarios
public async Task<List<ProductSummaryDto>> GetProductSummariesAsync(CancellationToken ct)
{
    return await _context.Products
        .AsNoTracking()
        .Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        })
        .ToListAsync(ct);
}

// ✅ Use Include for related data
public async Task<Product?> GetProductWithCategoryAsync(int id, CancellationToken ct)
{
    return await _context.Products
        .Include(p => p.Category)
        .FirstOrDefaultAsync(p => p.Id == id, ct);
}

// ✅ Use Split queries for multiple collections
public async Task<Product?> GetProductWithDetailsAsync(int id, CancellationToken ct)
{
    return await _context.Products
        .AsSplitQuery()
        .Include(p => p.Category)
        .Include(p => p.Reviews)
        .Include(p => p.Images)
        .FirstOrDefaultAsync(p => p.Id == id, ct);
}
```

### 11.2 Memory Management
```csharp
// ✅ Use IAsyncEnumerable for large datasets
public async IAsyncEnumerable<ProductDto> GetProductsStreamAsync([EnumeratorCancellation] CancellationToken ct = default)
{
    await foreach (var product in _context.Products.AsAsyncEnumerable().WithCancellation(ct))
    {
        yield return _mapper.Map<ProductDto>(product);
    }
}

// ✅ Dispose resources properly
public async Task ProcessLargeFileAsync(Stream fileStream, CancellationToken ct)
{
    using var reader = new StreamReader(fileStream);
    await using var writer = new StreamWriter("output.txt");
    
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
        ct.ThrowIfCancellationRequested();
        await writer.WriteLineAsync(line);
    }
}
```

---

## 12  Deployment & Configuration

### 12.1 Environment Configuration
```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "#{ConnectionString}#"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "FusionCache": {
    "DefaultEntryOptions": {
      "Duration": "00:05:00",
      "JitterMaxDuration": "00:00:30"
    }
  }
}
```

---

## 13  Code Quality Rules

### 13.1 SonarQube Rules
- **Cognitive Complexity**: Max 15 per method
- **Cyclomatic Complexity**: Max 10 per method
- **Method Length**: Max 50 lines
- **Class Length**: Max 500 lines
- **Parameter Count**: Max 7 parameters

### 13.2 Code Analysis
```xml
<!-- In Directory.Build.props -->
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <WarningsAsErrors />
  <WarningsNotAsErrors>CS1591</WarningsNotAsErrors>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <AnalysisMode>All</AnalysisMode>
</PropertyGroup>
```

---

## 14  Forbidden Practices

### ❌ Never Do This:
```csharp
// ❌ Don't use .Result or .Wait()
var result = SomeAsyncMethod().Result;

// ❌ Don't mix layers
@code {
    public async Task LoadProduct(int id)
    {
        // ❌ Direct DbContext access in UI layer
        var product = await _context.Products.FindAsync(id);
        // UI logic mixed with data access
    }
}

// ❌ Don't use magic strings
await _cache.GetAsync("product-list-all");

// ❌ Don't ignore exceptions
try 
{
    await SomeOperation();
} 
catch { } // ❌ Empty catch

// ❌ Don't use blocking operations in async methods
public async Task<string> ReadFileAsync()
{
    return File.ReadAllText("file.txt"); // ❌ Blocking I/O
}
```

### ✅ Do This Instead:
```csharp
// ✅ Use async/await properly
var result = await SomeAsyncMethod();

// ✅ Follow layer separation
@code {
    public async Task LoadProduct(int id)
    {
        var result = await Mediator.Send(new GetProductByIdQuery(id));
        if (result.Succeeded)
        {
            Product = result.Data;
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
    }
}

// ✅ Use constants for cache keys
await _cache.GetAsync(CacheKeys.ProductList());

// ✅ Handle exceptions properly
try 
{
    await SomeOperation();
} 
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed");
    throw;
}

// ✅ Use async I/O operations
public async Task<string> ReadFileAsync()
{
    return await File.ReadAllTextAsync("file.txt");
}
```

---

## 15  Development Workflow

### 15.1 Before Committing
1. **Run Tests**: `dotnet test`
2. **Check Coverage**: Ensure >80% code coverage
3. **Run Analysis**: `dotnet build --verbosity normal`
4. **Format Code**: `dotnet format`
5. **Update Documentation**: XML docs for public APIs

### 15.2 Code Review Checklist
- [ ] Follows Clean Architecture principles
- [ ] Proper error handling and logging
- [ ] Input validation and security checks
- [ ] Performance considerations
- [ ] Tests cover new functionality
- [ ] Documentation updated
- [ ] No breaking changes without versioning

---

When uncertain about implementation details, Copilot should:

```csharp
// TODO: Clarify business rules with domain experts
// TODO: Verify performance requirements for this operation
// TODO: Confirm security requirements for this endpoint
```

This ensures human review for critical decisions.
