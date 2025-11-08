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
    public const string Key = nameof(AISettings);

    /// <summary>
    /// Gets or sets the Gemini API key
    /// </summary>
    public string GeminiApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API key used to authenticate requests to the OpenAI service.
    /// </summary>
    /// <remarks>The API key must be valid and authorized for the intended OpenAI operations. Storing
    /// sensitive credentials such as API keys should be done securely to prevent unauthorized access.</remarks>
    public string OpenAIApiKey { get; set; } = string.Empty;
} 
