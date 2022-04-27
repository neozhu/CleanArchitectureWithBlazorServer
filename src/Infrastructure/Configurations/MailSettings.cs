// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

public class MailSettings
{
    public const string SectionName = nameof(MailSettings);
    public string From { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = false;
}
