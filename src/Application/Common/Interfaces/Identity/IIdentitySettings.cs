namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

public interface IIdentitySettings
{
    // Password settings.
    /// <summary>
    ///     Gets or sets a value indicating whether a password should require a digit.
    /// </summary>
    bool RequireDigit { get; }

    /// <summary>
    ///     Gets or sets a value indicating what the minimum required length of a password should be.
    /// </summary>
    int RequiredLength { get; }

    /// <summary>
    ///     Gets or sets a value indicating what the maximum required length of a password should be.
    /// </summary>
    int MaxLength { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether the password should require a non-alphanumeric(not: 0-9, A-Z) character.
    /// </summary>
    bool RequireNonAlphanumeric { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether a password should require an upper-case character.
    /// </summary>
    bool RequireUpperCase { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether a password should require an lower-case character.
    /// </summary>
    bool RequireLowerCase { get; }

    // Lockout settings.
    /// <summary>
    ///     Gets or sets a value indicating what the default lockout TimeSpan should be, measured in minutes.
    /// </summary>
    int DefaultLockoutTimeSpan { get; }
}