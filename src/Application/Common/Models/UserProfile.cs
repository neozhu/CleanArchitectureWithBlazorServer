namespace CleanArchitecture.Blazor.Application.Common.Models;

public class UserProfile
{
    public string? Provider { get; set; }
    public string? SuperiorName { get; set; }
    public string? SuperiorId { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? DisplayName { get; set; }
    public required string UserName { get; set; }  
    public required string Email { get; set; }  
    public string? PhoneNumber { get; set; }
    public string? DefaultRole { get; set; }
    public string[]? AssignedRoles { get; set; }
    public required string UserId { get; set; }=Guid.NewGuid().ToString();
    public bool IsActive { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }

}

public class UserProfileEditValidator : AbstractValidator<UserProfile>
{
    public UserProfileEditValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(128)
            .NotEmpty();
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(128);
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<UserProfile>.CreateWithOptions((UserProfile)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}