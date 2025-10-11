namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Service for managing tenant switching functionality
/// </summary>
public interface ITenantSwitchService
{
    
    
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
    
    
}

 
