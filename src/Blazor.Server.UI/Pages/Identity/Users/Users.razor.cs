using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using Blazor.Server.UI;
using Blazor.Server.UI.Shared;
using MudBlazor;
using MediatR;
using Blazor.Server.UI.Components.Shared;
using Blazor.Server.UI.Components.Shared.Themes;
using Blazor.Server.UI.Components.Index;
using Blazor.Server.UI.Components.Charts;
using Blazor.Server.UI.Models.Localization;
using FluentValidation;
using CleanArchitecture.Blazor.Infrastructure.Identity;
using CleanArchitecture.Blazor.Application.Constants.Permission;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Blazor.Server.UI.Components.Common;
using Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Identity;

namespace Blazor.Server.UI.Pages.Identity.Users
{
    public partial class Users:IDisposable
    {
        private List<ApplicationUser> UserList = new List<ApplicationUser>();
        private HashSet<ApplicationUser> SelectItems = new HashSet<ApplicationUser>();
        private string _searchString = string.Empty;
        private bool _sortNameByLength;
        public string? Title { get; private set; }

        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default !;
        [Inject]
        private UserManager<ApplicationUser> _userManager { get; set; } = default !;
        [Inject]
        public CircuitHandler circuitHandler { get; set; } = default !;
        private bool _canCreate;
        private bool _canSearch;
        private bool _canEdit;
        private bool _canDelete;
        private bool _canActive;
        private bool _canManageRoles;
        private bool _canRestPassword;
        private bool _canImport;
        private bool _canExport;
        private bool _loading;
        private MudTable<ApplicationUser> _table = default !;
        protected override async Task OnInitializedAsync()
        {
            Title = L["Users"];
            var state = await AuthState;
            _canCreate = (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Create)).Succeeded;
            _canSearch = (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Search)).Succeeded;
            _canEdit = (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Edit)).Succeeded;
            _canDelete = (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Delete)).Succeeded;
            _canActive = (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Active)).Succeeded;
            _canManageRoles = (await AuthService.AuthorizeAsync(state.User, Permissions.Users.ManageRoles)).Succeeded;
            _canRestPassword = (await AuthService.AuthorizeAsync(state.User, Permissions.Users.RestPassword)).Succeeded;
            _canImport = false; // (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Import)).Succeeded;
            _canExport = false; // (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Export)).Succeeded;
            UserList = await _userManager.Users.ToListAsync();
            (circuitHandler as CircuitHandlerService).CircuitsChanged += HandleCircuitsChanged;
        }
        public void Dispose()
        {
            (circuitHandler as CircuitHandlerService).CircuitsChanged -= HandleCircuitsChanged;
        }
        public void HandleCircuitsChanged(object sender, bool connected)
        {
            InvokeAsync(async () =>
            {
                await OnRefresh();
                StateHasChanged();
            });
        }

        private Func<ApplicationUser, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (x.UserName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (x.DisplayName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{x.Email} {x.PhoneNumber} {x.Site}".Contains(_searchString))
                return true;
            return false;
        };
        private async Task OnRefresh()
        {
            UserList = await _userManager.Users.ToListAsync();
        }

        private async Task OnCreate()
        {
            var model = new UserFormModel();
            var parameters = new DialogParameters{["model"] = model};
            var options = new DialogOptions{CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true};
            var dialog = DialogService.Show<_UserFormDialog>(L["Create a new user"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var applicationUser = new ApplicationUser()
                {Site = model.Site, DisplayName = model.DisplayName, UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber, ProfilePictureDataUrl = model.ProfilePictureDataUrl};
                var password = model.Password;
                var state = await _userManager.CreateAsync(applicationUser, password);
                if (state.Succeeded)
                {
                    UserList.Add(applicationUser);
                    Snackbar.Add($"{L["Create successfully"]}", MudBlazor.Severity.Info);
                }
                else
                {
                    Snackbar.Add($"{string.Join(",", (state.Errors.Select(x => x.Description).ToArray()))}", MudBlazor.Severity.Error);
                }
            }
        }

        private async Task OnEdit(ApplicationUser item)
        {
            var roles = await _userManager.GetRolesAsync(item);
            var model = new UserFormModel()
            {Id = item.Id, Site = item.Site, DisplayName = item.DisplayName, UserName = item.UserName, Email = item.Email, PhoneNumber = item.PhoneNumber, ProfilePictureDataUrl = item.ProfilePictureDataUrl, AssignRoles = roles.ToArray()};
            var parameters = new DialogParameters{["model"] = model};
            var options = new DialogOptions{CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true};
            var dialog = DialogService.Show<_UserFormDialog>(L["Edit the user"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                item.DisplayName = model.DisplayName;
                item.UserName = model.UserName;
                item.Email = model.Email;
                item.PhoneNumber = model.PhoneNumber;
                item.ProfilePictureDataUrl = model.ProfilePictureDataUrl;
                var state = await _userManager.UpdateAsync(item);
                if (model.AssignRoles is not null && model.AssignRoles.Length > 0)
                {
                    await _userManager.RemoveFromRolesAsync(item, roles);
                    await _userManager.AddToRolesAsync(item, model.AssignRoles);
                }

                if (state.Succeeded)
                {
                    Snackbar.Add($"{L["Update successfully"]}", MudBlazor.Severity.Info);
                }
                else
                {
                    Snackbar.Add($"{string.Join(",", (state.Errors.Select(x => x.Description).ToArray()))}", MudBlazor.Severity.Error);
                }
            }
        }

        private async Task OnDeleteChecked()
        {
            string deleteContent = L["You're sure you want to delete selected items:{0}?"];
            var parameters = new DialogParameters{{nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, SelectItems.Count)}};
            var options = new DialogOptions{CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true};
            var dialog = DialogService.Show<DeleteConfirmation>(L["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                foreach (var item in SelectItems)
                {
                    await _userManager.DeleteAsync(item);
                    UserList.Remove(item);
                }
            }
        }

        private async Task OnSetActive(ApplicationUser item)
        {
            item.IsActive = !item.IsActive;
            var state = await _userManager.UpdateAsync(item);
            if (state.Succeeded)
            {
                Snackbar.Add($"{L["Update successfully"]}", MudBlazor.Severity.Info);
            }
            else
            {
                Snackbar.Add($"{string.Join(",", (state.Errors.Select(x => x.Description).ToArray()))}", MudBlazor.Severity.Error);
            }
        }

        private async Task OnResetPassword(ApplicationUser item)
        {
            var model = new ResetPasswordFormModel()
            {Id = item.Id, DisplayName = item.DisplayName, UserName = item.UserName, ProfilePictureDataUrl = item.ProfilePictureDataUrl};
            var parameters = new DialogParameters{["model"] = model};
            var options = new DialogOptions{CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall};
            var dialog = DialogService.Show<_ResetPasswordDialog>(L["Set Password"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                item.DisplayName = model.DisplayName;
                item.UserName = model.UserName;
                item.ProfilePictureDataUrl = model.ProfilePictureDataUrl;
                var token = await _userManager.GeneratePasswordResetTokenAsync(item);
                var state = await _userManager.ResetPasswordAsync(item, token, model.Password);
                if (state.Succeeded)
                {
                    Snackbar.Add($"{L["Reset password successfully"]}", MudBlazor.Severity.Info);
                }
                else
                {
                    Snackbar.Add($"{string.Join(",", (state.Errors.Select(x => x.Description).ToArray()))}", MudBlazor.Severity.Error);
                }
            }
        }

        private Task OnExport()
        {
            return Task.CompletedTask;
        }

        private Task OnImportData(InputFileChangeEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}