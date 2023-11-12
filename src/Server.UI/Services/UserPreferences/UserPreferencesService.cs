// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;

public interface IUserPreferencesService
{
    /// <summary>
    /// Saves UserPreferences in local storage
    /// </summary>
    /// <param name="userPreferences">The userPreferences to save in the local storage</param>
    public Task SaveUserPreferences(UserPreferences userPreferences);

    /// <summary>
    /// Loads UserPreferences in local storage
    /// </summary>
    /// <returns>UserPreferences object. Null when no settings were found.</returns>
    public Task<UserPreferences> LoadUserPreferences();
}

public class UserPreferencesService : IUserPreferencesService
{
    private readonly ProtectedLocalStorage _localStorage;
    private const string Key = "userPreferences";

    public UserPreferencesService(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveUserPreferences(UserPreferences userPreferences)
    {
        await _localStorage.SetAsync(Key, userPreferences);
    }

    public async Task<UserPreferences> LoadUserPreferences()
    {
        try
        {
            var result = await _localStorage.GetAsync<UserPreferences>(Key);
            if (result.Success && result.Value is not null)
            {
                return result.Value;
            }
            return new UserPreferences();
        }
        catch (CryptographicException)
        {
            await _localStorage.DeleteAsync(Key);
            return new UserPreferences();
        }

    }
}

