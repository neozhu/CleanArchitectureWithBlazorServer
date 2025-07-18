# Common AI Prompts for Clean Architecture Development

## **Feature Creation Prompts**

### **Complete Feature Implementation**
```
Create a complete {FeatureName} feature following Clean Architecture pattern:

1. **Domain Entity**: Create {Entity} entity in Domain/Entities/ with properties: {list properties}
2. **Application Layer**:
   - {Entity}Dto in Application/Features/{Feature}/DTOs/
   - Create{Entity}Command with validator in Commands/ folder
   - Get{Entity}ByIdQuery with caching in Queries/ folder  
   - Get{Entity}sWithPaginationQuery in Queries/ folder
   - AutoMapper profile for {Entity} mappings
   - {Feature}AccessRights class in Application/Common/Security/AccessRights/
   - Permissions for {Feature} in Application/Common/Security/Permissions/
3. **Infrastructure Layer**:
   - EF Core configuration for {Entity} in Infrastructure/Persistence/Configurations/
4. **UI Layer**:
   - {Feature}.razor page with MudDataGrid in Server.UI/Pages/{Feature}/
   - {Entity}FormDialog component for create/edit operations
   - Proper authorization using Permissions.{Feature}.*

Please follow the existing project patterns and naming conventions.
```

### **CQRS Command Creation**
```
Create a {CommandName}Command following CQRS pattern in Application/Features/{Feature}/Commands/:

- Command record with properties: {list properties}
- CommandHandler with proper error handling and logging
- FluentValidation validator with rules: {list validation rules}
- Use ICacheInvalidatorRequest<Result<{ReturnType}>> interface
- Include proper exception handling and Result pattern
- Add XML documentation

Follow the existing project patterns for {similar command example}.
```

### **CQRS Query Creation**
```
Create a {QueryName}Query following CQRS pattern in Application/Features/{Feature}/Queries/:

- Query record implementing ICacheableRequest<Result<{ReturnType}>>
- QueryHandler using ProjectTo for efficient mapping
- Cache key: "{Entity}:{parameters}" with {duration} expiry
- Include AsNoTracking() for read-only operations
- Use pagination if returning multiple items
- Add XML documentation

Follow the existing project patterns for {similar query example}.
```

## **Component Creation Prompts**

### **Blazor Page Creation**
```
Create a {FeatureName}.razor page following the project patterns:

1. **Page Structure**:
   - Route: /pages/{feature-name}
   - Authorization: Permissions.{Feature}.View
   - Use MudDataGrid with ServerData
   - Include toolbar with Create/Delete/Export buttons
   - Conditional rendering based on permissions

2. **Required Injections**:
   - IStringLocalizer<{Feature}> L
   - IMediator Mediator
   - ISnackbar Snackbar
   - IPermissionService PermissionService

3. **Features**:
   - Server-side pagination and sorting
   - Multi-selection support
   - Loading states
   - Error handling with snackbar notifications
   - Proper localization

Follow the existing Products.razor or Contacts.razor patterns.
```

### **Dialog Component Creation**
```
Create a {Entity}FormDialog component for create/edit operations:

1. **Component Features**:
   - Inherit from MudDialogBase
   - Support both create and edit modes
   - Use MudForm with FluentValidation
   - Include Save/Cancel buttons
   - Show loading states during operations

2. **Required Parameters**:
   - _model: AddEdit{Entity}Command
   - Refresh: Func<Task> callback

3. **Form Fields**:
   {list form fields with appropriate MudBlazor components}

4. **Validation**:
   - Use existing AddEdit{Entity}CommandValidator
   - Show validation errors inline

Follow the existing dialog patterns in the project.
```

## **Service Implementation Prompts**

### **Application Service Interface**
```
Create I{ServiceName}Service interface in Application/Common/Interfaces/:

- Methods: {list methods with signatures}
- Use Result<T> pattern for return types
- Include CancellationToken parameters
- Add proper XML documentation
- Follow existing service interface patterns

Then create implementation in Infrastructure/Services/ and register in Infrastructure.DependencyInjection.
```

### **Infrastructure Service Implementation**
```
Create {ServiceName}Service in Infrastructure/Services/ implementing I{ServiceName}Service:

1. **Constructor Dependencies**:
   - Required services: {list dependencies}
   - ILogger<{ServiceName}Service>

2. **Implementation Requirements**:
   - Proper error handling with try-catch
   - Structured logging with context
   - Use Result<T> pattern for returns
   - Include async/await with ConfigureAwait(false)

3. **Registration**:
   - Add to Infrastructure.DependencyInjection as Scoped service

Follow existing service patterns like UserService or TenantService.
```

## **Configuration Management Prompts**

### **Typed Configuration Creation**
```
Create typed configuration for {ConfigurationName}:

1. **Interface**: I{ConfigurationName}Settings in Application/Common/Interfaces/
   - Properties: {list properties with types}

2. **Implementation**: {ConfigurationName}Settings in Infrastructure/Configurations/
   - Implement interface
   - Include Key constant
   - Set default values

3. **Registration**: Add to Infrastructure.DependencyInjection:
   - Configure with IOptions pattern
   - Register both concrete class and interface

4. **Usage**: Inject I{ConfigurationName}Settings in services

Follow the pattern of IApplicationSettings/AppConfigurationSettings.
```

## **Database Migration Prompts**

### **Entity Configuration**
```
Create EF Core configuration for {Entity} in Infrastructure/Persistence/Configurations/:

1. **Configuration Class**: {Entity}Configuration : IEntityTypeConfiguration<{Entity}>
2. **Table Mapping**: 
   - Table name: {tableName}
   - Primary key configuration
   - Property configurations: {list properties with constraints}
   - Index definitions: {list indexes}
   - Relationships: {list relationships}

3. **Migration**: Generate migration with name: Add{Entity}Table

Follow existing configuration patterns like ProductConfiguration.
```

## **Testing Prompts**

### **Unit Test Creation**
```
Create unit tests for {ClassName} in tests/Application.UnitTests/:

1. **Test Class**: {ClassName}Tests with NUnit framework
2. **Setup**: Mock dependencies using Moq
3. **Test Cases**:
   - Happy path scenarios
   - Error conditions
   - Edge cases
   - Validation scenarios

4. **Assertions**: Use FluentAssertions for readable assertions
5. **Arrange-Act-Assert**: Follow AAA pattern consistently

Test methods to include: {list specific test scenarios}
```

### **Integration Test Creation**
```
Create integration tests for {FeatureName} in tests/Application.IntegrationTests/:

1. **Test Class**: {FeatureName}IntegrationTests inheriting from TestBase
2. **Test Scenarios**:
   - Full CRUD operations
   - Permission validation
   - Data persistence verification
   - Cache behavior testing

3. **Setup**: Use TestBase for database setup and cleanup
4. **Data**: Create test data using entity builders

Follow existing integration test patterns.
```

## **Refactoring Prompts**

### **Architecture Compliance Fix**
```
Fix architecture violation in {FileName}:

Current issue: {describe the violation}

Required changes:
1. Move {specific code} to appropriate layer
2. Create interface in Application layer if needed
3. Update dependency injection registration
4. Ensure proper dependency direction: UI → Application → Domain

Maintain existing functionality while following Clean Architecture principles.
```

### **Performance Optimization**
```
Optimize performance for {ComponentName}:

1. **Database Queries**:
   - Add AsNoTracking() for read-only operations
   - Use ProjectTo<TDto>() for efficient mapping
   - Implement proper pagination
   - Add appropriate caching with FusionCache

2. **Component Rendering**:
   - Add @key directives for list items
   - Implement ShouldRender() if needed
   - Minimize state changes

3. **Memory Usage**:
   - Dispose resources properly
   - Use IAsyncEnumerable for large datasets

Maintain existing functionality while improving performance.
```

## **Security Enhancement Prompts**

### **Permission System Integration**
```
Add proper permission controls to {FeatureName}:

1. **Permission Definitions**: Add to Application/Common/Security/Permissions/{Feature}.cs:
   - View, Create, Edit, Delete, Export, Import permissions

2. **AccessRights Class**: Create {Feature}AccessRights in Application/Common/Security/AccessRights/

3. **Page Authorization**: Add [Authorize(Policy = Permissions.{Feature}.View)] to pages

4. **Conditional UI**: Use permission checks for button visibility

5. **API Security**: Ensure handlers validate permissions appropriately

Follow existing permission patterns in the project.
```

## **Localization Prompts**

### **Multi-language Support**
```
Add localization support for {FeatureName}:

1. **Resource Files**: Create in Server.UI/Resources/Pages/{Feature}/
   - {Feature}.resx (default/English)
   - {Feature}.zh-CN.resx (Chinese)
   - Additional languages as needed

2. **Component Integration**:
   - Inject IStringLocalizer<{Feature}> L
   - Replace hardcoded strings with L["Key"]
   - Add meaningful resource keys

3. **Resource Keys**: {list specific strings that need localization}

Follow existing localization patterns in the project.
```

## **Documentation Prompts**

### **API Documentation**
```
Generate comprehensive XML documentation for {ClassName}:

1. **Class Summary**: Describe purpose and responsibilities
2. **Method Documentation**:
   - Summary of what each method does
   - Parameter descriptions with types and constraints
   - Return value descriptions
   - Exception documentation
   - Example usage where helpful

3. **Property Documentation**: Document public properties with their purpose

Follow existing documentation standards in the project.
```

---

## **Usage Instructions**

1. **Replace Placeholders**: Update {FeatureName}, {Entity}, etc. with actual values
2. **Provide Context**: Include specific requirements and constraints
3. **Reference Existing Code**: Point to similar implementations in the project
4. **Specify Layers**: Clearly indicate which layer you're working in
5. **Include Validation Rules**: Specify business rules and validation requirements

These prompts are designed to work with Cursor AI to generate code that follows the project's Clean Architecture principles and established patterns. 