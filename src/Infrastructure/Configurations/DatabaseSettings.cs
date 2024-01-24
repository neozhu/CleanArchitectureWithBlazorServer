using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

/// <summary>
///     Configuration wrapper for the database section
/// </summary>
public class DatabaseSettings : IValidatableObject
{
    /// <summary>
    ///     Database key constraint
    /// </summary>
    public const string Key = nameof(DatabaseSettings);

    /// <summary>
    ///     Represents the database provider, which to connect to
    /// </summary>
    public string DBProvider { get; set; } = string.Empty;

    /// <summary>
    ///     The connection string being used to connect with the given database provider
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    ///     Validates the entered configuration
    /// </summary>
    /// <param name="validationContext">Describes the context in which a validation check is performed.</param>
    /// <returns>The result of the validation</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(DBProvider))
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(DBProvider)} is not configured",
                new[] { nameof(DBProvider) });

        if (string.IsNullOrEmpty(ConnectionString))
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured",
                new[] { nameof(ConnectionString) });
    }
}