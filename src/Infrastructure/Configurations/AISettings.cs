// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

/// <summary>
/// AI configuration settings implementation
/// </summary>
public class AISettings : IAISettings
{
    /// <summary>
    /// AI configuration key constraint
    /// </summary>
    public const string Key = "AI";

    /// <summary>
    /// Gets or sets the Gemini API key
    /// </summary>
    public string GeminiApiKey { get; set; } = string.Empty;
} 