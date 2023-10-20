using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Tenants.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Queries.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Tenants;
public partial class Tenants
{
    public string? Title { get; private set; }
    private int _defaultPageSize = 15;
    private HashSet<TenantDto> _selectedItems = new HashSet<TenantDto>();
    private MudDataGrid<TenantDto> _table = default!;
    private TenantDto _currentDto = new();
    private string _searchString = string.Empty;

    private bool _loading;
    [Inject]
    private IMediator Mediator { get; set; } = default!;
    [Inject]
    private IMapper Mapper { get; set; } = default!;
    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    private TenantsWithPaginationQuery Query { get; set; } = new();

    private bool _canSearch;
    private bool _canCreate;
    private bool _canEdit;
    private bool _canDelete;

    protected override async Task OnInitializedAsync()
    {
        Title = L[_currentDto.GetClassDescription()];
        var state = await AuthState;
        _canCreate = (await AuthService.AuthorizeAsync(state.User, Permissions.Tenants.Create)).Succeeded;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, Permissions.Tenants.Search)).Succeeded;
        _canEdit = (await AuthService.AuthorizeAsync(state.User, Permissions.Tenants.Edit)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, Permissions.Tenants.Delete)).Succeeded;
    }
    private async Task<GridData<TenantDto>> ServerReload(GridState<TenantDto> state)
    {
        try
        {
            _loading = true;
            Query.Keyword = _searchString;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;
            var result = await Mediator.Send(Query);
            return new GridData<TenantDto>() { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }

    }

    private async Task OnSearch(string text)
    {
        _selectedItems = new();
        _searchString = text;
        await _table.ReloadServerData();
    }
    private async Task OnRefresh()
    {
        TenantCacheKey.Refresh();
        _selectedItems = new();
        _searchString = string.Empty;
        await _table.ReloadServerData();
    }
    private async Task OnCreate()
    {
        var command = new AddEditTenantCommand();
        var parameters = new DialogParameters<_TenantFormDialog>
        {
            { x=>x.Model,command },
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = DialogService.Show<_TenantFormDialog>
        (L["Create a new Tenant"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
    private async Task OnEdit(TenantDto dto)
    {
        var command = Mapper.Map<AddEditTenantCommand>(dto);
        var parameters = new DialogParameters<_TenantFormDialog>
        {
            { x=>x.Model,command },
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = DialogService.Show<_TenantFormDialog>
        (L["Edit the Tenant"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task OnDelete(TenantDto dto)
    {
        var deleteContent = ConstantString.DeleteConfirmationWithId;
        var command = new DeleteTenantCommand(new string[] { dto.Id });
        var parameters = new DialogParameters<DeleteConfirmation>
                {
                    { x=>x.Command,  command },
                    { x=>x.ContentText, string.Format(deleteContent, dto.Id) }
                };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(L["Delete the Tenant"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task OnDeleteChecked()
    {
        var command = new DeleteTenantCommand(_selectedItems.Select(x => x.Id).ToArray());
        var deleteContent = ConstantString.DeleteConfirmation;
        var parameters = new DialogParameters<DeleteConfirmation>
                    {
                        { x=>x.Command,  command },
                        { x=>x.ContentText, string.Format(deleteContent,_selectedItems.Count) }
                    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(L["Delete Selected Tenants"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
            _selectedItems = new HashSet<TenantDto>();
        }
    }
}