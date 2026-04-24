using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
//// <summary>
/// Implementation of the IPermissionService using reflection to evaluate access rights
/// based on a naming convention. For example, an access rights model named "ContactsAccessRights"
/// will be associated with permission claims like "Permissions.Contacts.Create".
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IAuthorizationService _authService;
    private readonly AuthenticationStateProvider _authStateProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionService"/> class.
    /// </summary>
    /// <param name="authService">The authorization service used for permission checks.</param>
    /// <param name="authStateProvider">The provider for retrieving the current authentication state.</param>
    public PermissionService(IAuthorizationService authService, AuthenticationStateProvider authStateProvider)
    {
        _authService = authService;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Check if the current user has the specified permission
    /// </summary>
    /// <param name="permission">Permission to check</param>
    /// <returns>True if user has permission, false otherwise</returns>
    public async Task<bool> HasPermissionAsync(string permission)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var result = await _authService.AuthorizeAsync(user, permission);
        return result.Succeeded;
    }

    /// <summary>
    /// Get all permissions for the current user
    /// </summary>
    /// <returns>List of permissions</returns>
    public async Task<List<string>> GetUserPermissionsAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        var permissions = new List<string>();
        var allPermissions = GetAllPermissions();
        
        // Check each permission for the current user
        var tasks = allPermissions.Select(async permission =>
        {
            var result = await _authService.AuthorizeAsync(user, permission);
            return new { Permission = permission, HasPermission = result.Succeeded };
        });
        
        var results = await Task.WhenAll(tasks);
        permissions.AddRange(results.Where(r => r.HasPermission).Select(r => r.Permission));
        
        return permissions;
    }

    /// <summary>
    /// Get all available permissions in the system
    /// </summary>
    /// <returns>List of all registered permissions</returns>
    public List<string> GetAllPermissions()
    {
        var permissions = new List<string>();
        var permissionsType = typeof(Permissions);
        
        // Get all nested types (permission classes like Products, Contacts, etc.)
        var nestedTypes = permissionsType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
        
        foreach (var nestedType in nestedTypes)
        {
            // Get all constant string fields from each nested type
            var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                   .Where(f => f.IsLiteral && f.FieldType == typeof(string));
            
            foreach (var field in fields)
            {
                var value = field.GetValue(null) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    permissions.Add(value);
                }
            }
        }
        
        return permissions;
    }

    /// <summary>
    /// Retrieves the access rights for the specified access rights model type by checking each permission using reflection.
    /// The access rights model type must follow the naming convention where the type name ends with "AccessRights".
    /// For instance, "ContactsAccessRights" maps to permission claims like "Permissions.Contacts.Create".
    /// </summary>
    /// <typeparam name="TAccessRights">The type of the access rights model. Must have a parameterless constructor.</typeparam>
    /// <returns>
    /// An instance of TAccessRights with each boolean property set to true if the corresponding permission check succeeded, otherwise false.
    /// </returns>
    public async Task<TAccessRights> GetAccessRightsAsync<TAccessRights>() where TAccessRights : new()
    {
        // Retrieve the current authentication state.
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        var accessRightsResult = new TAccessRights();

        // Ensure the type name ends with "AccessRights" (e.g., "ContactsAccessRights").
        var typeName = typeof(TAccessRights).Name;
        if (!typeName.EndsWith("AccessRights", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("TAccessRights type name must end with 'AccessRights'");
        }

        // Extract the section name from the type name (e.g., "Contacts" from "ContactsAccessRights").
        var sectionName = typeName.Substring(0, typeName.Length - "AccessRights".Length);

        // Get all public instance properties of TAccessRights.
        var properties = typeof(TAccessRights).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Create a dictionary to hold tasks for checking permissions concurrently.
        var tasks = new Dictionary<PropertyInfo, Task<AuthorizationResult>>();

        foreach (var prop in properties)
        {
            // Only process boolean properties that are writable.
            if (prop.PropertyType == typeof(bool) && prop.CanWrite)
            {
                // Construct the permission claim string, e.g., "Permissions.Contacts.Create".
                var permissionClaim = $"Permissions.{sectionName}.{prop.Name}";
                // Start the permission check task for the given claim.
                tasks[prop] = _authService.AuthorizeAsync(user, permissionClaim);
            }
        }

        // Wait for all permission checks to complete concurrently.
        await Task.WhenAll(tasks.Values);

        // Assign the results to the corresponding properties in the access rights model.
        foreach (var kvp in tasks)
        {
            var property = kvp.Key;
            var authResult = kvp.Value.Result;
            property.SetValue(accessRightsResult, authResult.Succeeded);
        }

        return accessRightsResult;
    }
}
