namespace CleanArchitecture.Blazor.Application.Common.Interfaces;
/// <summary>
/// Interface for the permission service that provides a method to retrieve access rights
/// based on an access rights model type.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Retrieves the access rights for the given access rights model type by checking each permission
    /// against the current user's authorization state.
    /// </summary>
    /// <typeparam name="TAccessRights">
    /// The access rights model type. Its name must end with "AccessRights" (e.g., ContactsAccessRights),
    /// and its properties correspond to the permission names.
    /// </typeparam>
    /// <returns>
    /// An instance of TAccessRights where each boolean property indicates whether the current user has that permission.
    /// </returns>
    Task<TAccessRights> GetAccessRightsAsync<TAccessRights>() where TAccessRights : new();
}
