using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Permission;
using CleanArchitecture.Blazor.Server.UI.Components.Common;
using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using MudExtensions;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Identity.Roles;
public partial class Roles
{
    private bool _processing;
    private string _currentRoleName = string.Empty;
    private int _defaultPageSize = 15;
    private HashSet<ApplicationRoleDto> _selectedItems = new();
    private readonly ApplicationRoleDto _currentDto = new();
    private string _searchString = string.Empty;
    private string? Title { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    private RoleManager<ApplicationRole> RoleManager { get; set; } = default!;

    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);
    private LazyCacheEntryOptions Options => new LazyCacheEntryOptions().SetAbsoluteExpiration(RefreshInterval, ExpirationMode.LazyExpiration);

    [Inject]
    private IAppCache Cache { get; set; } = null!;

    private List<PermissionModel> _permissions = new();
    private IList<Claim> _assignedClaims = default!;
    private MudDataGrid<ApplicationRoleDto> _table = null!;
    private bool _canCreate;
    private bool _canSearch;
    private bool _canEdit;
    private bool _canDelete;
    private bool _canManagePermissions;
    private bool _canImport;
    private bool _canExport;
    private bool _showPermissionsDrawer;
    private bool _loading;
    private bool _uploading;

    protected override async Task OnInitializedAsync()
    {
        RoleManager = ScopedServices.GetRequiredService<RoleManager<ApplicationRole>>();
        Title = L[_currentDto.GetClassDescription()];
        var state = await AuthState;
        _canCreate = (await AuthService.AuthorizeAsync(state.User, Permissions.Roles.Create)).Succeeded;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, Permissions.Roles.Search)).Succeeded;
        _canEdit = (await AuthService.AuthorizeAsync(state.User, Permissions.Roles.Edit)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, Permissions.Roles.Delete)).Succeeded;
        _canManagePermissions = (await AuthService.AuthorizeAsync(state.User, Permissions.Roles.ManagePermissions)).Succeeded;
        _canImport = false; // (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Import)).Succeeded;
        _canExport = false; // (await AuthService.AuthorizeAsync(state.User, Permissions.Users.Export)).Succeeded;
    }

    private async Task<GridData<ApplicationRoleDto>> ServerReload(GridState<ApplicationRoleDto> state)
    {
        try
        {
            _loading = true;
            var items = await RoleManager.Roles
                .Where(x => x.Name!.ToLower().Contains(_searchString) || x.Description!.ToLower().Contains(_searchString))
                .EfOrderBySortDefinitions(state)
                .Skip(state.Page * state.PageSize).Take(state.PageSize)
                .Select(x => new ApplicationRoleDto
                { Id = x.Id, Name = x.Name!, Description = x.Description, NormalizedName = x.NormalizedName })
                .ToListAsync();
            var total = await RoleManager.Roles.CountAsync(x =>
                x.Name!.ToLower().Contains(_searchString) || x.Description!.ToLower().Contains(_searchString));
            return new GridData<ApplicationRoleDto> { TotalItems = total, Items = items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSearch(string text)
    {
        if (_loading) return;
        _searchString = text.ToLower();
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        await _table.ReloadServerData();
    }

    private async Task OnCreate()
    {
        var model = new ApplicationRoleDto { Name = "" };
        var parameters = new DialogParameters<_RoleFormDialog> {
        { x=>x.Model, model}
    };
        var options = new DialogOptions { CloseButton = true, CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = DialogService.Show<_RoleFormDialog>(L["Create a new role"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var applicationRole = new ApplicationRole
            {
                Name = model.Name,
                Description = model.Description
            };

            var state = await RoleManager.CreateAsync(applicationRole);
            if (state.Succeeded)
            {
                Snackbar.Add($"{ConstantString.CreateSuccess}", Severity.Info);
                await OnRefresh();
            }
            else
            {
                Snackbar.Add($"{string.Join(",", state.Errors.Select(x => x.Description).ToArray())}", Severity.Error);
            }
        }
    }

    private async Task OnEdit(ApplicationRoleDto item)
    {
        var parameters = new DialogParameters<_RoleFormDialog> {
        { x=>x.Model, item}
    };
        var options = new DialogOptions { CloseButton = true, CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = DialogService.Show<_RoleFormDialog>(L["Edit the role"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var role = await RoleManager.FindByIdAsync(item.Id);
            if (role is not null)
            {
                role.Description = item.Description;
                role.Name = item.Name;
                var state = await RoleManager.UpdateAsync(role);
                if (state.Succeeded)
                {
                    Snackbar.Add($"{ConstantString.UpdateSuccess}", Severity.Info);
                    await OnRefresh();
                }
                else
                {
                    Snackbar.Add($"{string.Join(",", state.Errors.Select(x => x.Description).ToArray())}", Severity.Error);
                }
            }
        }
    }

    private async Task OnSetPermissions(ApplicationRoleDto item)
    {
        _showPermissionsDrawer = true;
        _permissions = new List<PermissionModel>();
        _currentRoleName = item.Name!;
        _permissions = await GetAllPermissions(item);
    }

    private async Task<List<PermissionModel>> GetAllPermissions(ApplicationRoleDto dto)
    {
        async Task<IList<Claim>> GetClaims(string roleId)
        {
            var role = await RoleManager.FindByIdAsync(dto.Id) ?? throw new NotFoundException($"not found application role: {roleId}");
            var claims = await RoleManager.GetClaimsAsync(role);
            return claims;
        }

        var key = $"get-claims-by-{dto.Id}";
        _assignedClaims = await Cache.GetOrAddAsync(key, async () => await GetClaims(dto.Id), Options);
        var allPermissions = new List<PermissionModel>();
        var modules = typeof(Permissions).GetNestedTypes();
        foreach (var module in modules)
        {
            var moduleName = string.Empty;
            var moduleDescription = string.Empty;
            if (module.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
                moduleName = displayNameAttribute.DisplayName;

            if (module.GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                moduleDescription = descriptionAttribute.Description;

            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            allPermissions.AddRange(from field in fields
                                    select field.GetValue(null)
                into propertyValue
                                    where propertyValue is not null
                                    select propertyValue.ToString()
                into claimValue
                                    select new PermissionModel
                                    {
                                        RoleId = dto.Id,
                                        ClaimValue = claimValue,
                                        ClaimType = ApplicationClaimTypes.Permission,
                                        Group = moduleName,
                                        Description = moduleDescription,
                                        Assigned = _assignedClaims.Any(x => x.Value == claimValue)
                                    });
        }
        return allPermissions;
    }

    private Task OnOpenChangedHandler(bool state)
    {
        _showPermissionsDrawer = state;
        return Task.CompletedTask;
    }

    private async Task OnAssignChangedHandler(PermissionModel model)
    {
        try
        {
            _processing = true;
            var roleId = model.RoleId!;
            var role = await RoleManager.FindByIdAsync(roleId) ?? throw new NotFoundException($"Application role {roleId} Not Found.");
            model.Assigned = !model.Assigned;
            if (model is { Assigned: true, ClaimType: not null, ClaimValue: not null })
            {
                await RoleManager.AddClaimAsync(role, new Claim(model.ClaimType, model.ClaimValue));
                Snackbar.Add($"{L["Permission added successfully!"]}", Severity.Info);
            }
            else
            {
                var removed = _assignedClaims.FirstOrDefault(x => x.Value == model.ClaimValue);
                if (removed is not null)
                {
                    await RoleManager.RemoveClaimAsync(role, removed);
                }
                Snackbar.Add($"{L["Permission removed successfully!"]}", Severity.Info);
            }
            var key = $"get-claims-by-{role.Id}";
            Cache.Remove(key);
        }
        finally
        {
            _processing = false;
        }
    }

    private async Task OnAssignAllChangedHandler(List<PermissionModel> models)
    {
        try
        {
            _processing = true;
            var roleId = models.First().RoleId;
            var role = await RoleManager.FindByIdAsync(roleId!) ?? throw new NotFoundException($"not found application role: {roleId}");
            foreach (var model in models)
            {
                if (model.Assigned)
                {
                    if (model.ClaimType is not null && model.ClaimValue is not null)
                    {
                        await RoleManager.AddClaimAsync(role, new Claim(model.ClaimType, model.ClaimValue));
                    }
                }
                else
                {
                    var removed = _assignedClaims.FirstOrDefault(x => x.Value == model.ClaimValue);
                    if (removed is not null)
                    {
                        await RoleManager.RemoveClaimAsync(role, removed);
                    }
                }
            }
            Snackbar.Add($"{L["Authorization has been changed"]}", Severity.Info);
            await Task.Delay(300);
            var key = $"get-claims-by-{role.Id}";
            Cache.Remove(key);
        }
        finally
        {
            _processing = false;
        }
    }

    private async Task OnDelete(ApplicationRoleDto dto)
    {
        var deleteContent = ConstantString.DeleteConfirmation;
        var parameters = new DialogParameters<ConfirmationDialog>
    {
        { x=>x.ContentText, string.Format(deleteContent, dto.Name) }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<ConfirmationDialog>(ConstantString.DeleteConfirmationTitle, parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var deleteRoles = await RoleManager.Roles.Where(x => x.Id == dto.Id).ToListAsync();
            foreach (var role in deleteRoles)
            {
                var deleteResult = await RoleManager.DeleteAsync(role);
                if (!deleteResult.Succeeded)
                {
                    Snackbar.Add($"{string.Join(",", deleteResult.Errors.Select(x => x.Description).ToArray())}", Severity.Error);
                    return;
                }
            }
            Snackbar.Add($"{ConstantString.DeleteSuccess}", Severity.Info);
            await OnRefresh();
        }
    }

    private async Task OnDeleteChecked()
    {
        var deleteContent = ConstantString.DeleteConfirmation;
        var parameters = new DialogParameters<ConfirmationDialog>
    {
        {x=> x.ContentText, string.Format(deleteContent, _selectedItems.Count) }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<ConfirmationDialog>(ConstantString.DeleteConfirmationTitle, parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var deleteId = _selectedItems.Select(x => x.Id).ToArray();
            var deleteRoles = await RoleManager.Roles.Where(x => deleteId.Contains(x.Id)).ToListAsync();
            foreach (var role in deleteRoles)
            {
                var deleteResult = await RoleManager.DeleteAsync(role);
                if (!deleteResult.Succeeded)
                {
                    Snackbar.Add($"{string.Join(",", deleteResult.Errors.Select(x => x.Description).ToArray())}", Severity.Error);
                    return;
                }
            }
            Snackbar.Add($"{ConstantString.DeleteSuccess}", Severity.Info);
            await OnRefresh();
        }
    }

    private Task OnExport()
    {
        return Task.CompletedTask;
    }

    private Task OnImportData(IBrowserFile file)
    {
        _uploading = true;
        _uploading = false;
        return Task.CompletedTask;
    }



}