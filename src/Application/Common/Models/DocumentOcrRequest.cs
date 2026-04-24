namespace CleanArchitecture.Blazor.Application.Common.Models;

public sealed record DocumentOcrRequest(int DocumentId, string? UserId,string? UserName,string? TenantId);
