# Cursor AI Configuration for Clean Architecture

This directory contains Cursor AI configuration files designed to enhance development productivity while maintaining Clean Architecture principles.

## 📁 **File Structure**

```
.cursor/
├── README.md                    # This documentation
├── rules/
│   ├── clean-architecture.md   # Core architecture rules and patterns
│   └── development-workflows.md # Common development workflows and templates
└── prompts/
    └── common-prompts.md       # Pre-built prompts for common tasks
```

## 🎯 **Configuration Files**

### **Root Level**
- **`.cursorrules`** - Main configuration file with comprehensive Clean Architecture rules

### **Rules Directory**
- **`clean-architecture.md`** - Detailed architecture compliance rules, validation checklists, and violation fixes
- **`development-workflows.md`** - Step-by-step workflows, code templates, and common patterns

### **Prompts Directory**  
- **`common-prompts.md`** - Ready-to-use AI prompts for feature creation, refactoring, and optimization

## 🚀 **Getting Started**

### **1. Understanding the Architecture**
Before coding, review the layer dependency rules:
```
✅ ALLOWED: UI → Application → Domain
✅ ALLOWED: Infrastructure → Application → Domain
❌ FORBIDDEN: Any reverse dependencies
```

### **2. Using Pre-built Prompts**
For common tasks, copy prompts from `.cursor/prompts/common-prompts.md` and customize:

**Example: Creating a new feature**
```
Create a complete OrderManagement feature following Clean Architecture pattern:

1. **Domain Entity**: Create Order entity in Domain/Entities/ with properties: Id, CustomerId, OrderDate, Status, Items
2. **Application Layer**:
   - OrderDto in Application/Features/Orders/DTOs/
   - CreateOrderCommand with validator in Commands/ folder
   - GetOrderByIdQuery with caching in Queries/ folder
   [... continue with customized requirements]
```

### **3. Code Generation Workflow**
1. **Plan**: Identify the layer and pattern needed
2. **Prompt**: Use appropriate template from prompts directory
3. **Generate**: Let Cursor AI create the code
4. **Validate**: Check against architecture rules
5. **Test**: Ensure functionality works as expected

## 🔍 **Architecture Validation**

### **Before Committing - Check:**
- [ ] No direct dependencies from UI to Infrastructure (except Program.cs)
- [ ] All services have interfaces in Application layer
- [ ] No direct DbContext access in UI components
- [ ] No direct IConfiguration injection in UI
- [ ] All constants are in Application.Common.Constants
- [ ] All permissions are in Application.Common.Security
- [ ] Proper error handling with Result pattern
- [ ] XML documentation for public APIs

### **Common Violations and Fixes**

| Violation | Fix |
|-----------|-----|
| `@inject ApplicationDbContext` in UI | Use `@inject IMediator Mediator` with CQRS |
| `@inject IConfiguration Config` in UI | Create typed settings interface |
| `using Infrastructure.Services` in UI | Create interface in Application layer |
| Direct permission strings | Use constants from Permissions class |

## 📋 **Development Patterns**

### **Service Implementation**
1. Create interface in `Application/Common/Interfaces/`
2. Implement in `Infrastructure/Services/`
3. Register in `Infrastructure.DependencyInjection`
4. Inject interface in UI components

### **CQRS Implementation**
1. Commands in `Application/Features/{Feature}/Commands/`
2. Queries in `Application/Features/{Feature}/Queries/`
3. Include validators for all commands/queries
4. Use caching for read operations

### **UI Components**
1. Pages in `Server.UI/Pages/{Feature}/`
2. Components in `Server.UI/Pages/{Feature}/Components/`
3. Use proper authorization attributes
4. Follow MudBlazor patterns

## 💡 **Best Practices**

### **When Working with Cursor AI:**
1. **Be Specific**: Reference exact layers and patterns
2. **Provide Context**: Mention similar existing implementations
3. **Include Constraints**: Specify business rules and validation requirements
4. **Reference Rules**: Point to specific sections in the rules files
5. **Validate Output**: Always check generated code against architecture rules

### **Example Good Prompt:**
```
Create a ProductReviewService interface in Application/Common/Interfaces/ following the existing service patterns like IUserService. Include methods for GetReviewsByProductId, CreateReview, and UpdateReview. All methods should use Result<T> pattern and include CancellationToken. Then implement in Infrastructure/Services/ with proper error handling and logging.
```

### **Example Poor Prompt:**
```
Create a review service
```

## 🛠️ **Troubleshooting**

### **Architecture Violations**
If Cursor generates code that violates architecture rules:
1. Check the `.cursorrules` file is being applied
2. Be more specific about layer requirements in your prompt
3. Reference existing compliant implementations
4. Use the fix patterns from `rules/clean-architecture.md`

### **Permission Issues**
If generated code lacks proper authorization:
1. Reference the permission system documentation
2. Include authorization requirements in your prompt
3. Check existing pages for permission patterns

### **Performance Concerns**
If generated code has performance issues:
1. Use the optimization prompts from `prompts/common-prompts.md`
2. Reference database query optimization patterns
3. Include caching requirements in your prompts

## 📚 **Additional Resources**

- **Project Documentation**: See main README.md for project overview
- **Architecture Guide**: Review REFACTOR-PLAN.md for completed improvements
- **Code Examples**: Examine existing features like Products, Users, or Contacts

## 🔄 **Updating Rules**

When you discover new patterns or violations:
1. Add new rules to appropriate files in `rules/` directory
2. Create new prompt templates in `prompts/` directory
3. Update the main `.cursorrules` file if needed
4. Document changes in this README

---

**Remember**: These rules are designed to maintain code quality and architectural integrity. Always prioritize following the patterns over speed of development. 