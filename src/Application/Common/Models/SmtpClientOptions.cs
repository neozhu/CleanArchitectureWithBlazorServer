// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

/// <summary>
/// Configuration options for SMTP client
/// </summary>
public class SmtpClientOptions
{
    /// <summary>
    /// SMTP server host
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Whether to use SSL/TLS
    /// </summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>
    /// Username for authentication
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Password for authentication
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Default from email address
    /// </summary>
    public string DefaultFromEmail { get; set; } = string.Empty;


    /// <summary>
    /// Enable SMTP authentication
    /// </summary>
    public bool RequireCredentials { get; set; } = true;
}
