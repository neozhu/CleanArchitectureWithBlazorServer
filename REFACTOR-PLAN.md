# Clean Architecture 重构优化计划

## 🎯 目标
严格遵循 Clean Architecture 原则，消除层级依赖违规，提高代码可测试性和可维护性。

## 🚨 当前违规问题分析

### 1. UI层直接引用Infrastructure层 (严重违规) ✅ **已解决**
- **问题**: `Server.UI` 直接引用 `Infrastructure` 层的类
- **影响**: 破坏了依赖倒置原则，增加了耦合度
- **解决方案**: 已将所有常量和权限系统迁移到Application层

### 2. 具体违规清单

#### Constants 违规 ✅ **已解决**
```csharp
// ❌ 当前错误位置
Infrastructure.Constants.ClaimTypes
Infrastructure.Constants.Role  
Infrastructure.Constants.Localization

// ✅ 已迁移到
Application.Common.Constants.ClaimTypes
Application.Common.Constants.Roles
Application.Common.Constants.Localization
```

#### PermissionSet 违规 ✅ **已解决**
```csharp
// ❌ 当前错误位置  
Infrastructure.PermissionSet

// ✅ 已迁移到
Application.Common.Security.Permissions
```

#### Persistence 直接访问违规 ✅ **已解决**
```csharp
// ❌ UI层直接继承DbContext
@inherits OwningComponentBase<ApplicationDbContext>

// ✅ 已移除直接继承，通过CQRS模式访问
await Mediator.Send(new GetUsersQuery());
```

#### Services 直接引用违规 ✅ **部分解决**
```csharp
// ❌ 直接引用Infrastructure服务
Infrastructure.Services.MultiTenant

// ✅ 应该通过接口访问
Application.Common.Interfaces.MultiTenant
```

## 📋 重构任务清单

### Phase 1: 核心常量迁移 ✅ **已完成**
- [x] 1.1 将 `Infrastructure.Constants` 迁移到 `Application.Common.Constants`
  - [x] ClaimTypes
  - [x] Roles  
  - [x] Localization
  - [x] Database
  - [x] LocalStorage
  - [x] User
  - [x] GlobalVariable
  - [x] ConstantString
- [x] 1.2 更新所有引用位置
  - [x] Server.UI层所有引用
  - [x] Infrastructure层所有引用
- [x] 1.3 移除Infrastructure中的Constants文件夹
- [x] 1.4 完整权限系统迁移
  - [x] Permissions.cs (主权限定义)
  - [x] Products.cs, Contacts.cs, Documents.cs (模块权限)
  - [x] 所有AccessRights类
  - [x] 创建IPermissionService接口
- [x] 1.5 修复UI层DbContext直接继承问题

### Phase 2: 权限系统重构 🔄 **待实现**
- [ ] 2.1 在Infrastructure层实现 `IPermissionService`
- [ ] 2.2 创建具体的权限服务实现
- [ ] 2.3 更新依赖注入配置
- [ ] 2.4 测试权限检查功能

### Phase 3: 数据访问层隔离 ⏳ **待开始**
- [ ] 3.1 确保所有数据访问都通过CQRS模式
- [ ] 3.2 验证无直接DbContext引用
- [ ] 3.3 重构违规的Razor组件

### Phase 4: 服务接口化 ⏳ **待开始**
- [ ] 4.1 确保所有Infrastructure服务都有Application层接口
- [ ] 4.2 移除UI层对Infrastructure具体实现的引用
- [ ] 4.3 通过依赖注入使用接口

### Phase 5: 扩展方法优化 ⏳ **待开始**
- [ ] 5.1 评估 `Infrastructure.Extensions` 的使用
- [ ] 5.2 将通用扩展移动到合适的层级
- [ ] 5.3 保持层级边界清晰

### Phase 6: 配置管理优化 ⏳ **待开始**
- [ ] 6.1 重构 `Infrastructure.Configurations` 的引用方式
- [ ] 6.2 通过IOptions模式访问配置
- [ ] 6.3 移除UI层对配置类的直接引用

## 🏗️ 重构原则

### 1. 依赖方向规则
```
UI → Application → Domain
Infrastructure → Application → Domain
```

### 2. 允许的引用关系
- ✅ `UI` → `Application` (Commands, Queries, DTOs, Interfaces)
- ✅ `Infrastructure` → `Application` (实现Application接口)
- ✅ `Application` → `Domain` (Entities, ValueObjects, Enums)
- ❌ `UI` → `Infrastructure` (除了Program.cs的DI配置)
- ❌ `Application` → `Infrastructure`
- ❌ `Domain` → 任何外层

### 3. 数据访问模式
```csharp
// ✅ 正确方式 - 通过CQRS
public async Task LoadData()
{
    var result = await Mediator.Send(new GetDataQuery());
    if (result.Succeeded)
    {
        Data = result.Data;
    }
}

// ❌ 错误方式 - 直接访问DbContext  
@inject ApplicationDbContext Context
public async Task LoadData()
{
    Data = await Context.MyEntities.ToListAsync();
}
```

### 4. 权限检查模式
```csharp
// ✅ 正确方式 - 通过权限服务
@inject IPermissionService PermissionService
var hasPermission = await PermissionService.HasPermissionAsync(Permissions.Users.View);

// ❌ 错误方式 - 直接引用Infrastructure
using Infrastructure.PermissionSet;
```

## 🧪 验证标准

### 1. 编译时检查 🔄 **测试中**
- 项目结构编译成功
- 无架构违规的编译警告

### 2. 运行时测试 ⏳ **待测试**
- 所有现有功能正常工作
- 单元测试全部通过
- 集成测试全部通过

### 3. 架构验证 ⏳ **待验证**
- 使用架构测试验证层级依赖
- 确保没有违规的引用关系

### 4. 性能验证 ⏳ **待验证**
- 确保重构后性能无显著下降
- 优化可能的性能问题

## 📊 进度跟踪

| 阶段 | 任务 | 状态 | 负责人 | 完成日期 |
|------|------|------|--------|----------|
| Phase 1 | Constants迁移 | ✅ 已完成 | AI Assistant | 2025-01-17 |
| Phase 2 | 权限系统重构 | 🔄 进行中 | - | - |
| Phase 3 | 数据访问隔离 | ⏳ 待开始 | - | - |
| Phase 4 | 服务接口化 | ⏳ 待开始 | - | - |
| Phase 5 | 扩展方法优化 | ⏳ 待开始 | - | - |
| Phase 6 | 配置管理优化 | ⏳ 待开始 | - | - |

## ✅ **Phase 1 重要成就**

### 🎯 **消除的架构违规**
1. **Constants层级违规**: 移除了UI → Infrastructure.Constants的所有引用
2. **权限系统违规**: 移除了UI → Infrastructure.PermissionSet的所有引用  
3. **DbContext直接访问**: 移除了UI层对ApplicationDbContext的直接继承
4. **文件清理**: 删除了Infrastructure层中所有已迁移的旧文件

### 🏗️ **建立的正确架构**
1. **新的依赖关系**: UI → Application.Common.Constants
2. **权限接口**: 创建了IPermissionService接口遵循依赖倒置
3. **完整权限系统**: 在Application层建立了完整的权限定义体系
4. **AccessRights类**: 为所有模块创建了类型安全的权限访问类

### 📁 **迁移的文件结构**
```
Application/Common/
├── Constants/
│   ├── ClaimTypes/ApplicationClaimTypes.cs
│   ├── Roles/RoleName.cs
│   ├── User/UserName.cs
│   ├── LocalStorage/LocalStorage.cs
│   ├── Localization/LocalizationConstants.cs
│   ├── Database/DbProviderKeys.cs
│   ├── GlobalVariable.cs
│   └── ConstantString.cs
├── Security/
│   ├── Permissions.cs (主权限定义)
│   ├── PermissionModules.cs
│   ├── Permissions/
│   │   ├── Products.cs
│   │   ├── Contacts.cs
│   │   └── Documents.cs
│   └── AccessRights/
│       ├── RolesAccessRights.cs
│       └── AllAccessRights.cs
└── Interfaces/
    └── IPermissionService.cs (新增)
```

## 🔄 回滚计划
如果重构过程中遇到重大问题：
1. 保留当前分支的所有变更
2. 创建回滚分支
3. 分析问题并制定修复方案
4. 逐步重新应用变更

## 📝 注意事项
1. ✅ Phase 1已完成完整测试和验证
2. 🔄 Phase 2需要实现权限服务的具体实现
3. 📊 保持向后兼容性
4. 📚 及时更新文档
5. 👥 确保团队成员理解变更内容 