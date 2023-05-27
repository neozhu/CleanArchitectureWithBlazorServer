// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Configurations;

/// <summary>
///     Configuration wrapper for the app configuration section
/// </summary>
public class AppConfigurationSettings
{
    /// <summary>
    ///     App configuration key constraint
    /// </summary>
    public const string Key = nameof(AppConfigurationSettings);
    
    /// <summary>
    ///     Contains the application secret, used for signing
    /// </summary>
    public string Secret { get; set; } = string.Empty;
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public bool BehindSSLProxy { get; set; }
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public string ProxyIP { get; set; } = string.Empty;
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public string ApplicationUrl { get; set; } = string.Empty;
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public bool Resilience { get; set; }
}