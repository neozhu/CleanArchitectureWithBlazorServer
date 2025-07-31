# UserProfile 状态管理重构总结

本重构将 `UserProfileStateService` 改造为符合 Blazor 状态管理最佳实践的解决方案。

## 重构要点

### 1. 不可变数据结构 (Single Source of Truth)
- **旧版本**: `UserProfile` 是可变类 (class)，容易出现"脏引用"问题
- **新版本**: `UserProfile` 改为不可变记录 (record)，确保单一事实源

```csharp
// 新的不可变 UserProfile record
public sealed record UserProfile(
    string UserId,
    string UserName,
    string Email,
    // ... 其他属性
)
{
    public static UserProfile Empty => new(
        UserId: string.Empty,
        UserName: string.Empty,
        Email: string.Empty
    );
}
```

### 2. 精准通知机制
- **旧版本**: `event Func<Task>? OnChange` - 异步事件，容易出现竞态条件
- **新版本**: `event EventHandler<UserProfile>? Changed` - 同步事件，直接传递新的快照

```csharp
// 新的事件定义
public event EventHandler<UserProfile>? Changed;

// 订阅方式
UserProfileState.Changed += (sender, userProfile) => {
    // 收到新的 UserProfile 快照
    InvokeAsync(StateHasChanged);
};
```

### 3. 并发安全
- 使用 `SemaphoreSlim` 保护 Initialize/Refresh 操作
- 防止并发加载导致的数据不一致

```csharp
private readonly SemaphoreSlim _semaphore = new(1, 1);

public async Task EnsureInitializedAsync(string userId, CancellationToken cancellationToken = default)
{
    await _semaphore.WaitAsync(cancellationToken);
    try
    {
        // 安全的初始化逻辑
    }
    finally
    {
        _semaphore.Release();
    }
}
```

### 4. 缓存控制优化
- **旧版本**: 使用 `UserName` 作为缓存键，不够稳定
- **新版本**: 使用 `UserId` 作为缓存键，更加稳定可靠

```csharp
private string GetApplicationUserCacheKey(string userId)
{
    return $"GetApplicationUserDto:UserId:{userId}";
}
```

### 5. 接口设计
新增 `IUserProfileState` 接口，明确职责分离：

```csharp
public interface IUserProfileState
{
    UserProfile Value { get; }
    event EventHandler<UserProfile>? Changed;
    
    Task EnsureInitializedAsync(string userId, CancellationToken cancellationToken = default);
    Task RefreshAsync(CancellationToken cancellationToken = default);
    void Set(UserProfile userProfile);
    void UpdateLocal(string? profilePictureDataUrl = null, ...);
    void ClearCache();
}
```

## 使用方式

### 1. 在布局组件中初始化

```razor
@using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity
@inject IUserProfileState UserProfileState

protected override async Task OnInitializedAsync()
{
    var authState = await AuthState;
    var userId = authState.User.GetUserId();
    
    if (!string.IsNullOrEmpty(userId))
    {
        await UserProfileState.EnsureInitializedAsync(userId);
        UserProfileState.Changed += OnUserProfileChanged;
    }
}

private void OnUserProfileChanged(object? sender, UserProfile userProfile)
{
    InvokeAsync(StateHasChanged);
}
```

### 2. 通过级联参数使用

```razor
<CascadingValue Value="UserProfileState.Value" Name="UserProfile">
    @ChildContent
</CascadingValue>
```

### 3. 在组件中消费

```razor
[CascadingParameter(Name = "UserProfile")] 
public UserProfile UserProfile { get; set; } = UserProfile.Empty;

// 或者直接注入服务
@inject IUserProfileState UserProfileState

// 访问当前值
var currentProfile = UserProfileState.Value;
```

### 4. 更新状态

```csharp
// 数据库更新后的本地状态同步
UserProfileState.UpdateLocal(
    profilePictureDataUrl: newPictureUrl,
    displayName: newDisplayName
);

// 或者设置完整的新状态
UserProfileState.Set(newUserProfile);

// 强制从数据库刷新
await UserProfileState.RefreshAsync();
```

## 生命周期管理

服务注册为 `Scoped`，每个 Blazor 电路/连接有一个实例：

```csharp
services.AddScoped<IUserProfileState, UserProfileStateService>();
```

## 优势

1. **不可变更新**: 使用 `record with` 表达式，易于比较和调试
2. **单一事实源**: 所有组件从同一个 Store 读取相同的快照
3. **精准通知**: 只有真正改变时才触发更新
4. **并发安全**: 防止并发操作导致的状态不一致
5. **缓存优化**: 使用稳定的 UserId 键，提供精确的缓存控制
6. **类型安全**: 强类型接口，编译时检查

## 迁移注意事项

1. 将所有 `UserProfileStateService` 注入改为 `IUserProfileState`
2. 将 `OnChange` 事件订阅改为 `Changed` 事件
3. 将 `InitializeAsync(userName)` 改为 `EnsureInitializedAsync(userId)`
4. 将 `RefreshAsync(userName)` 改为 `RefreshAsync()`
5. 将 `UpdateUserProfile(...)` 改为 `UpdateLocal(...)` 或 `Set(...)`
6. 使用 `Value` 属性获取当前状态，而非 `UserProfile` 属性

这个重构显著提升了状态管理的可靠性、性能和开发体验。
