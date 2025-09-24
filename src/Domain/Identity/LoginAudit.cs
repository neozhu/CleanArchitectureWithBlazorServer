using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Identity;
public class LoginAudit : BaseAuditableEntity
{
    public DateTime LoginTimeUtc { get; set; }
    public string UserId { get; set; }= string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? BrowserInfo { get; set; }
    public string? Region { get; set; }
    public string? Provider { get; set; }
    public bool Success { get; set; } = true;
}
