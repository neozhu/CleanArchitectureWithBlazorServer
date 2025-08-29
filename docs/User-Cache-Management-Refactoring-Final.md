# User Cache Management Refactoring Summary

## Overview
This document describes the refactoring of scattered user cache management across multiple services into a centralized, simplified approach.

## Problem Statement
- Cache keys for ApplicationUser and userManager-related operations were scattered across multiple services
- Inconsistent naming conventions for cache keys
- Duplicate cache clearing logic in multiple services
- Hard to maintain and track all cache operations

## Solution Architecture

### Final Implementation: Simplified Static Approach
Based on user requirements for simplicity ("保存结构简单"), we implemented a minimal, static-only approach:

#### UserCacheKeys.cs
A static class that centralizes all user-related cache key generation:

```csharp
public static class UserCacheKeys
{
    // Specific cache key methods
    public static string UserContext(string userId) => $"User:Context:{userId}";
    public static string UserProfile(string userId) => $"User:Profile:{userId}";
    public static string UserApplication(string userId) => $"User:Application:{userId}";
    public static string UserClaims(string userId) => $"User:Claims:{userId}";
    public static string UserRoles(string userId) => $"User:Roles:{userId}";
    public static string UserPermissions(string userId) => $"User:Permissions:{userId}";
    public static string RoleClaims(string roleId) => $"Role:Claims:{roleId}";
    
    // Helper methods
    public static string GetCacheKey(string userId, UserCacheType cacheType) { ... }
    public static string[] AllUserKeys(string userId) { ... }
}
```

## Usage Examples

### Getting Cache Keys
```csharp
// Get specific cache key
var cacheKey = UserCacheKeys.UserContext(userId);
var key = UserCacheKeys.GetCacheKey(userId, UserCacheType.Context);

// Get all cache keys for a user
var allKeys = UserCacheKeys.AllUserKeys(userId);
```

### Cache Operations
Services now handle cache operations directly with IFusionCache:

```csharp
// Clear single cache
var cacheKey = UserCacheKeys.GetCacheKey(userId, UserCacheType.Context);
await _fusionCache.RemoveAsync(cacheKey);

// Clear multiple caches
var cacheTypes = new[] { UserCacheType.Claims, UserCacheType.Permissions, UserCacheType.Context };
var tasks = cacheTypes.Select(cacheType =>
{
    var cacheKey = UserCacheKeys.GetCacheKey(userId, cacheType);
    return _fusionCache.RemoveAsync(cacheKey).AsTask();
});
await Task.WhenAll(tasks);
```

## Benefits of This Approach

1. **Simplicity**: Single static class with no interfaces or complex abstractions
2. **No Dependencies**: No need for DI registration or service injection
3. **Centralized Keys**: All cache keys defined in one place
4. **Consistency**: Standardized naming pattern "User:{Type}:{Id}"
5. **Flexibility**: Services can use IFusionCache directly for cache operations
6. **Performance**: No abstraction overhead, direct cache operations

## Migration Summary

### Files Updated
1. `UserCacheKeys.cs` - Simplified to key generation only
2. `UserContextLoader.cs` - Updated to use direct cache operations
3. `UserProfileState.cs` - Updated cache clearing logic
4. `UserPermissionAssignmentService.cs` - Updated to handle multiple cache types
5. `TenantSwitchService.cs` - Updated context cache clearing
6. `PermissionHelper.cs` - Already using the key generation methods

### Key Changes
- Removed all cache management methods from UserCacheKeys
- Services now use IFusionCache directly for cache operations
- Simplified background cache clearing with Task.Run
- Maintained all existing cache functionality while simplifying the API

This refactoring achieved the goal of consolidating scattered cache management while keeping the structure simple and maintainable.
