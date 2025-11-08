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
    /// <summary>
    /// Gets the API key used to authenticate requests to the OpenAI service.
    /// </summary>
    string OpenAIApiKey { get; }
    } 
