// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

public class AppConfigurationSettings
{
    public const string SectionName = nameof(AppConfigurationSettings);
    public string Secret { get; set; }=String.Empty;
    public bool BehindSSLProxy { get; set; } 
    public string ProxyIP { get; set; } = String.Empty;
    public string ApplicationUrl { get; set; } = String.Empty;
}
