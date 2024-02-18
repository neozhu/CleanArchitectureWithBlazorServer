// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

/// <summary>
///     Configuration wrapper for the app configuration section
/// </summary>
public class AppConfigurationSettings : IApplicationSettings
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

    /// <summary>
    ///     Application framework including the version
    /// </summary>

    public string AppFlavor { get; set; } = "Blazor .NET 7.0";

    /// <summary>
    ///     Application .NET version
    /// </summary>
    public string AppFlavorSubscript { get; set; } = ".NET 7";

    /// <summary>
    ///     The name of the company
    /// </summary>
    public string Company { get; set; } = "Company";

    /// <summary>
    ///     Copyright watermark
    /// </summary>
    public string Copyright { get; set; } = "@2023 Copyright";

    /// <summary>
    ///     Current application version
    /// </summary>
    public string Version { get; set; } = "1.3.0";

    /// <summary>
    ///     Application framework
    /// </summary>

    public string App { get; set; } = "Blazor";

    /// <summary>
    ///     The application name / title
    /// </summary>
    public string AppName { get; set; } = "Blazor Dashboard";
}