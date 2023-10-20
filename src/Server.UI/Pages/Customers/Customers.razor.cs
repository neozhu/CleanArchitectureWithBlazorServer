using AutoMapper;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Customers.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.Customers.Commands.Import;
using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Queries.Export;
using CleanArchitecture.Blazor.Application.Features.Customers.Queries.Pagination;
using CleanArchitecture.Blazor.Application.Features.Customers.Specifications;
using CleanArchitecture.Blazor.Infrastructure.Constants.Permission;
using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Server.UI.Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Customers;
public partial class Customers
{
    public string? Title { get; private set; }
    private int _defaultPageSize = 15;
    private HashSet<CustomerDto> _selectedItems = new HashSet<CustomerDto>();
    private MudDataGrid<CustomerDto> _table = default!;
    private CustomerDto _currentDto = new();
    private bool _loading;
    private bool _uploading;
    private bool _exporting;
    [Inject]
    private IState<UserProfileState> UserProfileState { get; set; } = null!;
    private UserProfile UserProfile => UserProfileState.Value.UserProfile;

    [Inject]
    private IMediator Mediator { get; set; } = default!;
    [Inject]
    private IMapper Mapper { get; set; } = default!;
    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    private CustomersWithPaginationQuery Query { get; set; } = new();
    [Inject]
    private IBlazorDownloadFileService BlazorDownloadFileService { get; set; } = null!;
    private bool _canSearch;
    private bool _canCreate;
    private bool _canEdit;
    private bool _canDelete;
    private bool _canImport;
    private bool _canExport;

    protected override async Task OnInitializedAsync()
    {
        Title = L[_currentDto.GetClassDescription()];
        var state = await AuthState;
        _canCreate = (await AuthService.AuthorizeAsync(state.User, Permissions.Customers.Create)).Succeeded;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, Permissions.Customers.Search)).Succeeded;
        _canEdit = (await AuthService.AuthorizeAsync(state.User, Permissions.Customers.Edit)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, Permissions.Customers.Delete)).Succeeded;
        _canImport = (await AuthService.AuthorizeAsync(state.User, Permissions.Customers.Import)).Succeeded;
        _canExport = (await AuthService.AuthorizeAsync(state.User, Permissions.Customers.Export)).Succeeded;
    }
    private async Task<GridData<CustomerDto>> ServerReload(GridState<CustomerDto> state)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;
            var result = await Mediator.Send(Query).ConfigureAwait(false);
            return new GridData<CustomerDto>() { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }

    }
    private async Task OnSearch(string text)
    {
        _selectedItems = new();
        Query.Keyword = text;
        await _table.ReloadServerData();
    }
    private async Task OnChangedListView(CustomerListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }
    private async Task OnRefresh()
    {
        CustomerCacheKey.Refresh();
        _selectedItems = new();
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }

    private async Task OnCreate()
    {
        var command = new AddEditCustomerCommand();
        var parameters = new DialogParameters<_CustomerFormDialog>
        {
            { x=>x.model,command },
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = DialogService.Show<_CustomerFormDialog>
        (L["Create a new item"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
    private async Task OnClone()
    {
        var copyitem = _selectedItems.First();
        var command = new AddEditCustomerCommand()
        {
            Name = copyitem.Name,
            Description = copyitem.Description,
        };
        var parameters = new DialogParameters<_CustomerFormDialog>
        {
            { x=>x.model,command },
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = DialogService.Show<_CustomerFormDialog>
        (L["Create a new item"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
            _selectedItems.Remove(copyitem);
        }
    }
    private async Task OnEdit(CustomerDto dto)
    {
        var command = Mapper.Map<AddEditCustomerCommand>(dto);
        var parameters = new DialogParameters<_CustomerFormDialog>
        {
            { x=>x.model,command },
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = DialogService.Show<_CustomerFormDialog>
        (L["Edit the item"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task OnDelete(CustomerDto dto)
    {
        var command = new DeleteCustomerCommand(new int[] { dto.Id });
        var parameters = new DialogParameters<DeleteConfirmation>
       {
         { x=>x.Command,  command },
         { x=>x.ContentText, string.Format(ConstantString.DeleteConfirmation, dto.Name) }
       };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(ConstantString.DeleteConfirmationTitle, parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
            _selectedItems.Remove(dto);
        }
    }

    private async Task OnDeleteChecked()
    {
        var command = new DeleteCustomerCommand(_selectedItems.Select(x => x.Id).ToArray());
        var parameters = new DialogParameters<DeleteConfirmation>
                    {
                         { x=>x.Command,  command },
                         { x=>x.ContentText, string.Format(ConstantString.DeleteConfirmWithSelected,_selectedItems.Count) }
                    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(ConstantString.DeleteConfirmationTitle, parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
            _selectedItems = new();
        }
    }

    private async Task OnExport()
    {
        _exporting = true;
        var request = new ExportCustomersQuery()
        {
            Keyword = Query.Keyword,
            CurrentUser = UserProfile,
            ListView = Query.ListView,
            OrderBy = _table.SortDefinitions.Values.FirstOrDefault()?.SortBy ?? "Id",
            SortDirection = (_table.SortDefinitions.Values.FirstOrDefault()?.Descending ?? true) ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString()
        };
        var result = await Mediator.Send(request);
        if (result.Succeeded)
        {
            var downloadresult = await BlazorDownloadFileService.DownloadFile($"{L["Customers"]}.xlsx", result.Data, contentType: "application/octet-stream");
            Snackbar.Add($"{ConstantString.ExportSuccess}", MudBlazor.Severity.Info);
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", MudBlazor.Severity.Error);
        }
        _exporting = false;
    }
    private async Task OnImportData(IBrowserFile file)
    {
        _uploading = true;
        var stream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(stream);
        var command = new ImportCustomersCommand(file.Name, stream.ToArray());
        var result = await Mediator.Send(command);
        if (result.Succeeded)
        {
            await _table.ReloadServerData();
            Snackbar.Add($"{ConstantString.ImportSuccess}", MudBlazor.Severity.Info);
        }
        else
        {
            foreach (var msg in result.Errors)
            {
                Snackbar.Add($"{msg}", MudBlazor.Severity.Error);
            }
        }
        _uploading = false;
    }

}