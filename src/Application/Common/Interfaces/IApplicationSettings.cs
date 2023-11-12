namespace CleanArchitecture.Blazor.Application.Common.Interfaces;
public interface IApplicationSettings
{
    /// <summary>
    ///     Current application version
    /// </summary>
    string Version { get; }

    /// <summary>
    ///     Application framework
    /// </summary>

    string App { get; }

    /// <summary>
    ///     The application name / title
    /// </summary>
    string AppName { get; }
}
