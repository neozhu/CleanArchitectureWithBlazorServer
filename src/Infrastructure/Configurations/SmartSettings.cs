// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Infrastructure.Configurations;

public class SmartSettings
{
    public const string SectionName = nameof(SmartSettings);

    public string Version { get; set; }
    public string App { get; set; }
    public string AppName { get; set; }
    public string AppFlavor { get; set; }
    public string AppFlavorSubscript { get; set; }
    public Theme Theme { get; set; }
    public Features Features { get; set; }
}
