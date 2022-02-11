// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

public class DashbordSettings
{
    public const string SectionName = nameof(DashbordSettings);

    public string Version { get; set; }="6.0.2";
    public string App { get; set; } = "Dashbord";
    public string AppName { get; set; } = "Admin Dashbord";
    public string AppFlavor { get; set; } = String.Empty;
    public string AppFlavorSubscript { get; set; } = String.Empty;

    public string Company { get; set; } = "Company";
    public string Copyright { get; set; } = "@2022 Copyright";
    public Theme Theme { get; set; } = default!;
    public Features Features { get; set; } = default!;
}
