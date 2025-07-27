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
    Task<List<TenantDto>> GetAvailableTenantsAsync(string userId);

    /// <summary>
    /// Switch user to specified tenant
    /// </summary>
    Task<Result> SwitchToTenantAsync(string userId, string tenantId);

    /// <summary>
    /// Check if user can switch to specified tenant
    /// </summary>
    Task<bool> CanSwitchToTenantAsync(string userId, string tenantId);

    /// <summary>
    /// Get role mappings for user when switching to target tenant
    /// </summary>
    Task<List<RoleMappingDto>> GetRoleMappingsAsync(string userId, string targetTenantId);
}