# Clean Architecture 重构优化计划

## 🎯 目标
严格遵循 Clean Architecture 原则，消除层级依赖违规，提高代码可测试性和可维护性。

## 🚨 当前违规问题分析

### 1. UI层直接引用Infrastructure层 (严重违规)
- **问题**: `Server.UI` 直接引用 `Infrastructure` 层的类
- **影响**: 破坏了依赖倒置原则，增加了耦合度

### 2. 具体违规清单

#### Constants 违规
```csharp
// ❌ 当前错误位置
Infrastructure.Constants.ClaimTypes
Infrastructure.Constants.Role  
Infrastructure.Constants.Localization

// ✅ 应该移动到
Application.Common.Constants.ClaimTypes
Application.Common.Constants.Roles
Application.Common.Constants.Localization
```

#### PermissionSet 违规
```csharp
// ❌ 当前错误位置  
Infrastructure.PermissionSet

// ✅ 应该移动到
Application.Common.Security.Permissions
```

#### Persistence 直接访问违规
```csharp
// ❌ UI层直接继承DbContext
@inherits OwningComponentBase<ApplicationDbContext>

// ✅ 应该通过CQRS模式访问
await Mediator.Send(new GetUsersQuery());
```

#### Services 直接引用违规
```csharp
// ❌ 直接引用Infrastructure服务
Infrastructure.Services.MultiTenant

// ✅ 应该通过接口访问
Application.Common.Interfaces.MultiTenant
```

## 📋 重构任务清单

### Phase 1: 核心常量迁移 🔄
- [ ] 1.1 将 `Infrastructure.Constants` 迁移到 `Application.Common.Constants`
  - [ ] ClaimTypes
  - [ ] Roles  
  - [ ] Localization
  - [ ] Database
  - [ ] LocalStorage
  - [ ] User
- [ ] 1.2 更新所有引用位置
- [ ] 1.3 移除Infrastructure中的Constants文件夹

### Phase 2: 权限系统重构 🔐
- [ ] 2.1 将 `Infrastructure.PermissionSet` 迁移到 `Application.Common.Security`
- [ ] 2.2 创建 `IPermissionService` 接口在Application层
- [ ] 2.3 实现 `PermissionService` 在Infrastructure层  
- [ ] 2.4 更新UI层的权限检查逻辑

### Phase 3: 数据访问层隔离 🗃️
- [ ] 3.1 移除UI层对 `ApplicationDbContext` 的直接引用
- [ ] 3.2 确保所有数据访问都通过CQRS模式
- [ ] 3.3 重构违规的Razor组件

### Phase 4: 服务接口化 🔌
- [ ] 4.1 确保所有Infrastructure服务都有Application层接口
- [ ] 4.2 移除UI层对Infrastructure具体实现的引用
- [ ] 4.3 通过依赖注入使用接口

### Phase 5: 扩展方法优化 🛠️
- [ ] 5.1 评估 `Infrastructure.Extensions` 的使用
- [ ] 5.2 将通用扩展移动到合适的层级
- [ ] 5.3 保持层级边界清晰

### Phase 6: 配置管理优化 ⚙️
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

### 1. 编译时检查
- 所有项目编译成功
- 无任何架构违规的编译警告

### 2. 运行时测试
- 所有现有功能正常工作
- 单元测试全部通过
- 集成测试全部通过

### 3. 架构验证
- 使用架构测试验证层级依赖
- 确保没有违规的引用关系

### 4. 性能验证
- 确保重构后性能无显著下降
- 优化可能的性能问题

## 📊 进度跟踪

| 阶段 | 任务 | 状态 | 负责人 | 完成日期 |
|------|------|------|--------|----------|
| Phase 1 | Constants迁移 | 🔄 进行中 | - | - |
| Phase 2 | 权限系统重构 | ⏳ 待开始 | - | - |
| Phase 3 | 数据访问隔离 | ⏳ 待开始 | - | - |
| Phase 4 | 服务接口化 | ⏳ 待开始 | - | - |
| Phase 5 | 扩展方法优化 | ⏳ 待开始 | - | - |
| Phase 6 | 配置管理优化 | ⏳ 待开始 | - | - |

## 🔄 回滚计划
如果重构过程中遇到重大问题：
1. 保留当前分支的所有变更
2. 创建回滚分支
3. 分析问题并制定修复方案
4. 逐步重新应用变更

## 📝 注意事项
1. 每个Phase完成后进行完整测试
2. 保持向后兼容性
3. 及时更新文档
4. 确保团队成员理解变更内容 