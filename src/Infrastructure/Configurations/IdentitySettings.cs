namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

public class IdentitySettings : IIdentitySettings
{
    /// <summary>
    ///     Identity settings key constraint
    /// </summary>
    public const string Key = nameof(IdentitySettings);

    // Password settings.
    /// <summary>
    ///     Gets or sets a value indicating whether a password should require a digit.
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating what the minimum required length of a password should be.
    /// </summary>
    public int RequiredLength { get; set; } = 6;

    /// <summary>
    ///     Gets or sets a value indicating what the maximum required length of a password should be.
    /// </summary>
    public int MaxLength { get; set; } = 16;

    /// <summary>
    ///     Gets or sets a value indicating whether the password should require a non-alphanumeric(not: 0-9, A-Z) character.
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether a password should require an upper-case character.
    /// </summary>
    public bool RequireUpperCase { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether a password should require an lower-case character.
    /// </summary>
    public bool RequireLowerCase { get; set; } = false;

    // Lockout settings.
    /// <summary>
    ///     Gets or sets a value indicating what the default lockout TimeSpan should be, measured in minutes.
    /// </summary>
    public int DefaultLockoutTimeSpan { get; set; } = 30;
}