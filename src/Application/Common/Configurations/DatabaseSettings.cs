using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Common.Configurations;
public class DatabaseSettings: IValidatableObject
{
    public const string SectionName = nameof(DatabaseSettings);
    public string DBProvider { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(DBProvider))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(DBProvider)} is not configured",
                new[] { nameof(DBProvider) });
        }

        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured",
                new[] { nameof(ConnectionString) });
        }
    }
}
