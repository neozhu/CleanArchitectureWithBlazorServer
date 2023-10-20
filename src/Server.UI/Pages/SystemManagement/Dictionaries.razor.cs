using AutoMapper;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Import;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.Export;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.PaginationQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Blazor.Server.UI.Pages.SystemManagement;
public partial class Dictionaries
{
    private MudDataGrid<KeyValueDto> _table = null!;
    public string Title { get; set; } = "Picklist";
    private IList<KeyValueDto> _keyValueList = new List<KeyValueDto>();
    private HashSet<KeyValueDto> _selectedItems = new HashSet<KeyValueDto>();
    private KeyValueDto SelectedItem { get; set; } = new();
    private KeyValueDto ElementBeforeEdit { get; set; } = new();

    private string _searchString = string.Empty;
    private Picklist? _searchPicklist = null;
    private int _defaultPageSize = 15;
    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    private IMapper Mapper { get; set; } = default!;
    [Inject]
    private IBlazorDownloadFileService BlazorDownloadFileService { get; set; } = null!;
    [Inject]
    private IMediator Mediator { get; set; } = default!;
    private KeyValuesWithPaginationQuery Query { get; set; } = new();
    private bool _canCreate;
    private bool _canSearch;
    private bool _canEdit;
    private bool _canDelete;
    private bool _canImport;
    private bool _canExport;
    private bool Readonly => !_canCreate || !_canEdit;
    private bool _loading;
    private bool _uploading;
    private bool _downloading;
    protected override async Task OnInitializedAsync()
    {
        Title = L[SelectedItem.GetClassDescription()];
        var state = await AuthState;
        _canCreate = (await AuthService.AuthorizeAsync(state.User, Permissions.Dictionaries.Create)).Succeeded;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, Permissions.Dictionaries.Search)).Succeeded;
        _canEdit = (await AuthService.AuthorizeAsync(state.User, Permissions.Dictionaries.Edit)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, Permissions.Dictionaries.Delete)).Succeeded;
        _canImport = (await AuthService.AuthorizeAsync(state.User, Permissions.Dictionaries.Import)).Succeeded;
        _canExport = (await AuthService.AuthorizeAsync(state.User, Permissions.Dictionaries.Export)).Succeeded;

    }
    private async Task<GridData<KeyValueDto>> ServerReload(GridState<KeyValueDto> state)
    {
        try
        {
            _loading = true;
            var request = new KeyValuesWithPaginationQuery()
            {
                Keyword = _searchString,
                Picklist = _searchPicklist,
                OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id",
                SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString(),
                PageNumber = state.Page + 1,
                PageSize = state.PageSize
            };
            var result = await Mediator.Send(request).ConfigureAwait(false);
            return new GridData<KeyValueDto>() { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }
    private async Task OnSearch(string text)
    {
        _searchString = text;
        await _table.ReloadServerData();
    }
    private async Task OnSearch(Picklist? val)
    {
        _searchPicklist = val;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        KeyValueCacheKey.Refresh();
        _searchString = string.Empty;
        await _table.ReloadServerData();
    }

    private void CommittedItemChanges(KeyValueDto item)
    {
        InvokeAsync(async () =>
         {
             var command = Mapper.Map<AddEditKeyValueCommand>(item);
             var result = await Mediator.Send(command);
             if (!result.Succeeded)
             {
                 Snackbar.Add($"{result.ErrorMessage}", MudBlazor.Severity.Error);
             }
             StateHasChanged();
         });
    }
    private async Task DeleteItem(KeyValueDto item)
    {
        var deleteContent = ConstantString.DeleteConfirmation;
        var command = new DeleteKeyValueCommand(new int[] { item.Id });
        var parameters = new DialogParameters<DeleteConfirmation>
    {
        { x=>x.Command,  command },
        { x=>x.ContentText, string.Format(deleteContent,item.Name) }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(ConstantString.DeleteConfirmationTitle, parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
    private async Task OnDeleteChecked()
    {
        var deleteContent = ConstantString.DeleteConfirmation;
        var parameters = new DialogParameters<DeleteConfirmation>
    {
        { x=>x.ContentText, string.Format(deleteContent,_selectedItems.Count) }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(ConstantString.DeleteConfirmationTitle, parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            var command = new DeleteKeyValueCommand(_selectedItems.Select(x => x.Id).ToArray());
            var result = await Mediator.Send(command);
            await _table.ReloadServerData();
            Snackbar.Add($"{ConstantString.DeleteSuccess}", MudBlazor.Severity.Info);
        }
    }
    private async Task OnCreate()
    {
        var command = new AddEditKeyValueCommand()
        {
            Name = SelectedItem.Name,
            Description = SelectedItem?.Description,
        };
        var parameters = new DialogParameters<_CreatePicklistDialog>
        {
          { x=>x.model,command },
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = DialogService.Show<_CreatePicklistDialog>
        (L["Create a new picklist"], parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }

    }

    private async Task OnExport()
    {
        _downloading = true;
        var request = new ExportKeyValuesQuery()
        {
            Keyword = _searchString,
        };
        var result = await Mediator.Send(request);
        var downloadResult = await BlazorDownloadFileService.DownloadFile($"{L["Picklist"]}.xlsx", result, contentType: "application/octet-stream");
        Snackbar.Add($"{ConstantString.ExportSuccess}", MudBlazor.Severity.Info);
        _downloading = false;

    }
    private async Task OnImportData(InputFileChangeEventArgs e)
    {
        _uploading = true;
        var stream = new MemoryStream();
        await e.File.OpenReadStream(GlobalVariable.MaxAllowedSize).CopyToAsync(stream);
        var command = new ImportKeyValuesCommand(e.File.Name, stream.ToArray());
        var result = await Mediator.Send(command);
        if (result.Succeeded)
        {
            await OnRefresh();
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