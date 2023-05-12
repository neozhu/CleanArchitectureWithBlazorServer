// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Configurations;

/// <summary>
///     Configuration wrapper for the dashboard section
/// </summary>
public class DashboardSettings
{
    /// <summary>
    ///     Dashboard key constraint
    /// </summary>
    public const string Key = nameof(DashboardSettings);

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
 
}
