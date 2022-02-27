using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.Server.UI.Models;
using Blazor.Server.UI.Models.SideMenu;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace Blazor.Server.UI.Components.Shared;

public partial class SideMenu
{
    private IEnumerable<MenuSectionModel> _menuSections = new List<MenuSectionModel>()
    {
        new MenuSectionModel
        {
            Title = "GENERAL",
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    Title = "App",
                    Icon = Icons.Material.Filled.Speed,
                    Href = "/"
                },
                new()
                {
                    Title = "E-Commerce",
                    Icon = Icons.Material.Filled.ShoppingCart,
                    Href = "/ecommerce",
                    PageStatus = PageStatus.Wip
                },
                new()
                {
                    Title = "Analytics",
                    Icon = Icons.Material.Filled.Analytics,
                    Href = "/analytics",
                    PageStatus = PageStatus.ComingSoon
                },
                new()
                {
                    Title = "Banking",
                    Icon = Icons.Material.Filled.Money,
                    Href = "/banking",
                    PageStatus = PageStatus.ComingSoon
                },
                new()
                {
                    Title = "Booking",
                    Icon = Icons.Material.Filled.CalendarToday,
                    Href = "/booking",
                    PageStatus = PageStatus.ComingSoon
                }
            }
        },

        new MenuSectionModel
        {
            Title = "MANAGEMENT",
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    IsParent = true,
                    Title = "User",
                    Icon = Icons.Material.Filled.Person,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Profile",
                            Href = "/user/profile",
                            PageStatus = PageStatus.ComingSoon
                        },
                        new()
                        {
                            Title = "Cards",
                            Href = "/user/cards",
                            PageStatus = PageStatus.ComingSoon
                        },
                        new()
                        {
                            Title = "List",
                            Href = "/user/list",
                            PageStatus = PageStatus.ComingSoon
                        }
                    }
                },
                new()
                {
                    IsParent = true,
                    Title = "Article",
                    Icon = Icons.Material.Filled.Article,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Posts",
                            Href = "/user/posts",
                            PageStatus = PageStatus.ComingSoon
                        },
                        new()
                        {
                            Title = "Post",
                            Href = "/user/post",
                            PageStatus = PageStatus.ComingSoon
                        },
                        new()
                        {
                            Title = "New Post",
                            Href = "/user/newpost",
                            PageStatus = PageStatus.ComingSoon
                        }
                    }
                }
            }
        }
    };

    [EditorRequired] [Parameter] public bool SideMenuDrawerOpen { get; set; } 
    [EditorRequired] [Parameter] public EventCallback<bool> SideMenuDrawerOpenChanged { get; set; }
    [EditorRequired] [Parameter] public UserModel User { get; set; } = default!;

    [Inject] private IMenuService _menuService { get; set; } = default!;
    protected override Task OnInitializedAsync()
    {
        _menuSections = _menuService.Features;
        return Task.CompletedTask;
    }
}