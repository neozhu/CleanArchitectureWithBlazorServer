namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// AI configuration settings interface
/// </summary>
public interface IAISettings
{
    /// <summary>
    /// Gets the Gemini API key
    /// </summary>
    string GeminiApiKey { get; }
} 