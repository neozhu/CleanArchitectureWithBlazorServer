using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace Blazor.Server.UI.Pages.Identity.Users;

public class OrgItem
{
    public string? id { get; set; }
    public string? area { get; set; }
    public string? imageUrl { get; set; }
    public bool isLoggedUser { get; set; }
    public string? name { get; set; }
    public string? office { get; set; }
    public string? parentId { get; set; }
    public string? positionName { get; set; }
    public string? profileUrl { get; set; }
    public string? size { get; set; }
    public string? tags { get; set; }
    public int _directSubordinates { get; set; }
    public int _totalSubordinates { get; set; }
}
 