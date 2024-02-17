namespace CleanArchitecture.Blazor.Application.Common.Models;

public class OrgItem
{
    public string? Id { get; set; }
    public string? Area { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsLoggedUser { get; set; }
    public string? Name { get; set; }
    public string? Office { get; set; }
    public string? ParentId { get; set; }
    public string? PositionName { get; set; }
    public string? ProfileUrl { get; set; }
    public string? Size { get; set; }
    public string? Tags { get; set; }
    public int DirectSubordinates { get; set; }
    public int TotalSubordinates { get; set; }
}