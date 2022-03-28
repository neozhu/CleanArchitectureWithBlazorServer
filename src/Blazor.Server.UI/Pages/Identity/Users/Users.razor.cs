using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using CleanArchitecture.Blazor.Infrastructure.Identity;
using CleanArchitecture.Blazor.Application.Constants.Permission;
using Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;

namespace Blazor.Server.UI.Pages.Identity.Users
{
    public partial class Users : IDisposable
    {
        private List<ApplicationUser> UserList = new List<ApplicationUser>();
        private HashSet<ApplicationUser> SelectItems = new HashSet<ApplicationUser>();
        private string _searchString = string.Empty;
        private bool _sortNameByLength;
        public string? Title { get; private set; }

        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        //[Inject]
        //private UserManager<ApplicationUser> Service { get; set; } = default !;
        [Inject]
        public CircuitHandler circuitHandler { get; set; } = default!;
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
        private MudTable<ApplicationUser> _table = default!;
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
            await LoadData();
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
                Snackbar.Add($"{L["Circuits changed."]}", MudBlazor.Severity.Info);
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

            await LoadData();

        }
        private async Task LoadData()
        {
            if (_loading) return;
            try
            {
                _loading = true;
                UserList = await Service.Users.ToListAsync();
            }
            finally
            {
                _loading = false;
            }
        }
        private async Task OnCreate()
        {
            var model = new UserFormModel() { AssignRoles = new string[] { RoleConstants.BasicRole } };
            var parameters = new DialogParameters { ["model"] = model };
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = DialogService.Show<_UserFormDialog>(L["Create a new user"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var applicationUser = new ApplicationUser()
                {
                    Site = model.Site,
                    DisplayName = model.DisplayName,
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    ProfilePictureDataUrl = model.ProfilePictureDataUrl,
                };
                var password = model.Password;
                var state = await Service.CreateAsync(applicationUser, password);

                if (state.Succeeded)
                {
                    if (model.AssignRoles is not null && model.AssignRoles.Length > 0)
                    {
                        await Service.AddToRolesAsync(applicationUser, model.AssignRoles);
                    }
                    else
                    {
                        await Service.AddToRoleAsync(applicationUser, RoleConstants.BasicRole);
                    }
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
            var roles = await Service.GetRolesAsync(item);
            var model = new UserFormModel()
            {
                Id = item.Id,
                Site = item.Site,
                DisplayName = item.DisplayName,
                UserName = item.UserName,
                Email = item.Email,
                PhoneNumber = item.PhoneNumber,
                ProfilePictureDataUrl = item.ProfilePictureDataUrl,
                AssignRoles = roles.ToArray()
            };
            var parameters = new DialogParameters { ["model"] = model };
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            var dialog = DialogService.Show<_UserFormDialog>(L["Edit the user"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {

                var user = await Service.FindByIdAsync(item.Id);
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.ProfilePictureDataUrl = model.ProfilePictureDataUrl;
                user.DisplayName = model.DisplayName;
                user.Site = model.Site;
                user.UserName = model.UserName;
                var state = await Service.UpdateAsync(user);
                if (model.AssignRoles is not null && model.AssignRoles.Length > 0)
                {
                    await Service.RemoveFromRolesAsync(user, roles);
                    await Service.AddToRolesAsync(user, model.AssignRoles);
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
            var parameters = new DialogParameters { { nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, SelectItems.Count) } };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<DeleteConfirmation>(L["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                foreach (var item in SelectItems)
                {
                    var user = await Service.FindByIdAsync(item.Id);
                    await Service.DeleteAsync(user);
                    UserList.Remove(item);
                }
            }
        }

        private async Task OnSetActive(ApplicationUser item)
        {

            var user = await Service.FindByIdAsync(item.Id);
            user.IsActive = !user.IsActive;
            var state = await Service.UpdateAsync(user);
            //item.IsActive = !item.IsActive;
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
            { Id = item.Id, DisplayName = item.DisplayName, UserName = item.UserName, ProfilePictureDataUrl = item.ProfilePictureDataUrl };
            var parameters = new DialogParameters { ["model"] = model };
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = DialogService.Show<_ResetPasswordDialog>(L["Set Password"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var user = await Service.FindByIdAsync(item.Id);
                var token = await Service.GeneratePasswordResetTokenAsync(user);
                var state = await Service.ResetPasswordAsync(user, token, model.Password);
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