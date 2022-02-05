// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Infrastructure.Configurations;

public class AppConfigurationSettings
{
    public string Secret { get; set; }
    public bool BehindSSLProxy { get; set; }
    public string ProxyIP { get; set; }
    public string ApplicationUrl { get; set; }
}
