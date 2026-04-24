using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Server.UI.Models.NavigationMenu;

public enum PageStatus
{
    [Display(Name ="Coming Soon")] ComingSoon,
    [Display(Name = "WIP")] Wip,
    [Display(Name = "New")] New,
    [Display(Name = "Completed")] Completed
}
