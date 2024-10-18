using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
/// <summary>
/// A custom role validator to enforce multi-tenant uniqueness for roles.
/// Ensures that role names are unique within a specific tenant.
/// </summary>
internal class MultiTenantRoleValidator(ApplicationDbContext context) : RoleValidator<ApplicationRole>
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Validates the role for uniqueness based on role name and tenant ID.
    /// </summary>
    /// <param name="manager">The <see cref="RoleManager{ApplicationRole}"/> to manage roles.</param>
    /// <param name="role">The role to validate.</param>
    /// <returns>
    /// An <see cref="IdentityResult"/> that represents the outcome of the validation.
    /// Returns <see cref="IdentityResult.Success"/> if the role is valid, or a failed result if it is not.
    /// </returns>
    public override async Task<IdentityResult> ValidateAsync(RoleManager<ApplicationRole> manager, ApplicationRole role)
    {
        var errors = new List<IdentityError>();
        var duplicateRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == role.Name && r.TenantId == role.TenantId);

        if (duplicateRole != null && duplicateRole.Id != role.Id)
        {
            errors.Add(new IdentityError
            {
                Code = "DuplicateRoleName",
                Description = $"Role name '{role.Name}' already exists in the tenant."
            });
        }

        if (errors.Count > 0)
        {
            return IdentityResult.Failed(errors.ToArray());
        }

        return IdentityResult.Success;
    }
}
