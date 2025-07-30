# UserContext 重构文档

## 概述

本次重构将原有的 `ICurrentUserAccessor` 替换为新的 `UserContext` 和 `IUserContextAccessor` 架构。

## 新架构组件

### 1. UserContext
```csharp
public sealed record UserContext(
    string UserId,
    string UserName,
    string? TenantId = null,
    string? Email = null,
    IReadOnlyList<string>? Roles = null,
    string? SuperiorId = null
);
```

### 2. IUserContextAccessor
- 使用 `AsyncLocal` 实现按调用链隔离
- 支持嵌套的 `Push/Pop` 操作
- 注册为 `Singleton`

### 3. IUserContextLoader
- 通过 `ClaimsPrincipal` 使用 `UserManager` 加载 `UserContext`
- 提供 `LoadAsync` 和 `LoadAndSetAsync` 方法

### 4. UserContextHubFilter
- SignalR Hub 过滤器
- 在连接建立时自动设置用户上下文

## 使用方法

### 基本用法

```csharp
// 在服务中注入
public class MyService
{
    private readonly IUserContextAccessor _userContextAccessor;

    public MyService(IUserContextAccessor userContextAccessor)
    {
        _userContextAccessor = userContextAccessor;
    }

    public void DoSomething()
    {
        var currentUser = _userContextAccessor.Current;
        if (currentUser != null)
        {
            // 使用用户信息
            var userId = currentUser.UserId;
            var userName = currentUser.UserName;
            var roles = currentUser.Roles;
        }
    }
}
```

### 嵌套上下文

```csharp
// 推入新的用户上下文
using (var scope = _userContextAccessor.Push(newUserContext))
{
    // 在这个作用域内，Current 返回 newUserContext
    var context = _userContextAccessor.Current;
    
    // 执行需要特定用户上下文的操作
    await DoSomethingWithUserContext();
}
// 作用域结束时自动弹出上下文
```

### 手动设置上下文

```csharp
// 设置当前用户上下文
_userContextAccessor.Set(userContext);

// 清除当前用户上下文
_userContextAccessor.Clear();
```

### 在 SignalR Hub 中使用

```csharp
public class MyHub : Hub
{
    private readonly IUserContextAccessor _userContextAccessor;

    public MyHub(IUserContextAccessor userContextAccessor)
    {
        _userContextAccessor = userContextAccessor;
    }

    public async Task SendMessage(string message)
    {
        var currentUser = _userContextAccessor.Current;
        if (currentUser != null)
        {
            await Clients.All.SendAsync("ReceiveMessage", currentUser.UserName, message);
        }
    }
}
```

## 迁移指南

### 从 ICurrentUserAccessor 迁移

**旧代码：**
```csharp
@inject ICurrentUserAccessor CurrentUserAccessor

var sessionInfo = CurrentUserAccessor.SessionInfo;
var userId = sessionInfo?.UserId;
```

**新代码：**
```csharp
@inject IUserContextAccessor UserContextAccessor

var userContext = UserContextAccessor.Current;
var userId = userContext?.UserId;
```

### 从 SessionInfo 迁移

**旧代码：**
```csharp
var sessionInfo = new SessionInfo(userId, userName, displayName, ipAddress, tenantId, profilePicture, status);
```

**新代码：**
```csharp
var userContext = new UserContext(userId, userName, tenantId, email, roles, superiorId);
```

> **注意：** `SessionInfo` 已被完全移除，所有 Fusion 组件现在直接使用 `UserContext`。

## 优势

1. **线程安全**：使用 `AsyncLocal` 确保线程安全
2. **调用链隔离**：每个异步调用链都有独立的用户上下文
3. **嵌套支持**：支持嵌套的上下文管理
4. **性能优化**：减少不必要的数据库查询
5. **类型安全**：使用强类型的 `UserContext` 记录
6. **向后兼容**：通过适配器保持现有代码的兼容性

## 注意事项

1. `IUserContextAccessor` 注册为 `Singleton`，但内部使用 `AsyncLocal` 确保线程安全
2. 在 SignalR 连接中，`UserContextHubFilter` 会自动设置用户上下文
3. 现有的 `ICurrentUserAccessor` 已被完全移除，所有代码现在使用新的 `IUserContextAccessor`
4. 新的 `UserContext` 不包含 `IPAddress` 和 `ProfilePictureDataUrl`，如果需要这些信息，可以扩展 `UserContext` 或使用其他方式获取 