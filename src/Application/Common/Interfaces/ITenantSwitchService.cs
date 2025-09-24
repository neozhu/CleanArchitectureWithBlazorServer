using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Service for managing tenant switching functionality
/// </summary>
public interface ITenantSwitchService
{
    /// <summary>
    /// Get list of available tenants for the specified user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of available tenants</returns>
    Task<List<TenantDto>> GetAvailableTenantsAsync();
    
    /// <summary>
    /// Switch user to specified tenant
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="tenantId">Target tenant ID</param>
    /// <returns>Result of the switch operation</returns>
    Task<Result> SwitchToTenantAsync(string userId, string tenantId);
    
    /// <summary>
    /// Check if user can switch to specified tenant
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="tenantId">Target tenant ID</param>
    /// <returns>True if user can switch to the tenant</returns>
    Task<bool> CanSwitchToTenantAsync(string userId, string tenantId);
    
    /// <summary>
    /// Get role mappings for user when switching to target tenant
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="targetTenantId">Target tenant ID</param>
    /// <returns>List of role mappings</returns>
    Task<List<RoleMappingDto>> GetRoleMappingsAsync(string userId, string targetTenantId);
}

/// <summary>
/// DTO for role mapping information
/// </summary>
public class RoleMappingDto
{
    /// <summary>
    /// Current role name in user's current tenant
    /// </summary>
    public string CurrentRoleName { get; set; } = string.Empty;
    
    /// <summary>
    /// Target role name in the new tenant
    /// </summary>
    public string? TargetRoleName { get; set; }
    
    /// <summary>
    /// Whether the role will be assigned in the new tenant
    /// </summary>
    public bool WillBeAssigned { get; set; }
    
    /// <summary>
    /// Target role ID in the new tenant
    /// </summary>
    public string? TargetRoleId { get; set; }
} 
