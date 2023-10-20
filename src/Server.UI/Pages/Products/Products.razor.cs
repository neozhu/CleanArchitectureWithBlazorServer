using AutoMapper;
using BlazorDownloadFile;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.Products.Commands.Import;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Export;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;
using CleanArchitecture.Blazor.Application.Features.Products.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Blazor.Server.UI.Pages.Products;
public partial class Products
{
    public string? Title { get; private set; }
    private HashSet<ProductDto> _selectedItems = new();
    private MudDataGrid<ProductDto> _table = default!;
    private ProductDto _currentDto = new();
    private bool _loading;
    private bool _uploading;
    private bool _exporting;
    private bool _pdfExporting;
    private int _defaultPageSize = 15;
    [Inject]
    private IState<UserProfileState> UserProfileState { get; set; } = null!;
    private UserProfile UserProfile => UserProfileState.Value.UserProfile;
    [Inject]
    private IMediator Mediator { get; set; } = default!;

    [Inject]
    private IMapper Mapper { get; set; } = default!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    private ProductsWithPaginationQuery Query { get; } = new();

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
        _canCreate = (await AuthService.AuthorizeAsync(state.User, Permissions.Products.Create)).Succeeded;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, Permissions.Products.Search)).Succeeded;
        _canEdit = (await AuthService.AuthorizeAsync(state.User, Permissions.Products.Edit)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, Permissions.Products.Delete)).Succeeded;
        _canImport = (await AuthService.AuthorizeAsync(state.User, Permissions.Products.Import)).Succeeded;
        _canExport = (await AuthService.AuthorizeAsync(state.User, Permissions.Products.Export)).Succeeded;
    }

    private async Task<GridData<ProductDto>> ServerReload(GridState<ProductDto> state)
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

            return new GridData<ProductDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }

    //private  async Task OnFilterChanged(string s)
    //{
    //      await _table.ReloadServerData();

    //}
    private void ConditionChanged(string s)
    {
        InvokeAsync(async () => { await _table.ReloadServerData(); });
    }

    private async Task OnSearch(string text)
    {
        throw new Exception("OnSearch error!");
        _selectedItems = new HashSet<ProductDto>();
        Query.Keyword = text;
        await _table.ReloadServerData();
    }

    private async Task OnChangedListView(ProductListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        ProductCacheKey.Refresh();
        _selectedItems = new HashSet<ProductDto>();
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }

    private async Task OnCreate()
    {
        var command = new AddEditProductCommand { Pictures = new List<ProductImage>() };
        var parameters = new DialogParameters<_ProductFormDialog>
    {
        { x=>x.Refresh , new Action(() => _table.ReloadServerData()) },
        { x=>x.Model, command }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = DialogService.Show<_ProductFormDialog>
            (string.Format(ConstantString.CreateAnItem, L["Product"]), parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
            await _table.ReloadServerData();
    }

    private async Task OnClone()
    {
        var copy = _selectedItems.First();
        var command = new AddEditProductCommand
        {
            Brand = copy.Brand,
            Description = copy.Description,
            Price = copy.Price,
            Unit = copy.Unit,
            Name = copy.Name,
            Pictures = copy.Pictures
        };
        var parameters = new DialogParameters<_ProductFormDialog>
    {
        { x=>x.Refresh , new Action(() => _table.ReloadServerData()) },
        { x=>x.Model, command }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = DialogService.Show<_ProductFormDialog>
            (string.Format(ConstantString.CreateAnItem, L["Product"]), parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
            await _table.ReloadServerData();
    }

    private async Task OnEdit(ProductDto dto)
    {
        var command = Mapper.Map<AddEditProductCommand>(dto);
        var parameters = new DialogParameters<_ProductFormDialog>
    {
        { x=>x.Refresh , new Action(() => _table.ReloadServerData()) },
        { x=>x.Model, command }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = DialogService.Show<_ProductFormDialog>
            (string.Format(ConstantString.EditTheItem, L["Product"]), parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
            await _table.ReloadServerData();
    }

    private async Task OnDelete(ProductDto dto)
    {
        var command = new DeleteProductCommand(new[] { dto.Id });
        var parameters = new DialogParameters<DeleteConfirmation>
    {
        { x=>x.Command ,command },
        { x=>x.ContentText, string.Format(ConstantString.DeleteConfirmation, dto.Name) }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(string.Format(ConstantString.EditTheItem, L["Product"]), parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task OnDeleteChecked()
    {
        var command = new DeleteProductCommand(_selectedItems.Select(x => x.Id).ToArray());
        var parameters = new DialogParameters<DeleteConfirmation>
    {
        { x=>x.Command, command },
        { x=>x.ContentText, string.Format(ConstantString.DeleteConfirmWithSelected, _selectedItems.Count) }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>
            (string.Format(ConstantString.DeleteTheItem, L["Product"]), parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            await _table.ReloadServerData();
            _selectedItems = new HashSet<ProductDto>();
        }
    }

    private async Task OnExport()
    {
        _exporting = true;
        var request = new ExportProductsQuery
        {
            Brand = Query.Brand,
            Name = Query.Name,
            MinPrice = Query.MinPrice,
            MaxPrice = Query.MaxPrice,
            Unit = Query.Unit,
            Keyword = Query.Keyword,
            ListView = Query.ListView,
            OrderBy = _table.SortDefinitions.Values.FirstOrDefault()?.SortBy ?? "Id",
            SortDirection = _table.SortDefinitions.Values.FirstOrDefault()?.Descending ?? false ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString(),
            ExportType = ExportType.Excel
        };
        var result = await Mediator.Send(request);
        if (result.Succeeded)
        {
            var downloadResult = await BlazorDownloadFileService.DownloadFile($"{L["Products"]}.xlsx", result.Data, "application/octet-stream");
            Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
        }
        _exporting = false;
    }

    private async Task OnExportPDF()
    {
        _pdfExporting = true;
        var request = new ExportProductsQuery
        {
            Brand = Query.Brand,
            Name = Query.Name,
            MinPrice = Query.MinPrice,
            MaxPrice = Query.MaxPrice,
            Unit = Query.Unit,
            Keyword = Query.Keyword,
            ListView = Query.ListView,
            OrderBy = _table.SortDefinitions.Values.FirstOrDefault()?.SortBy ?? "Id",
            SortDirection = _table.SortDefinitions.Values.FirstOrDefault()?.Descending ?? false ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString(),
            ExportType = ExportType.PDF
        };
        var result = await Mediator.Send(request);
        if (result.Succeeded)
        {
            var downloadResult = await BlazorDownloadFileService.DownloadFile($"{L["Products"]}.pdf", result.Data, "application/octet-stream");
            Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
        }
        _pdfExporting = false;
    }

    private async Task OnImportData(IBrowserFile file)
    {
        _uploading = true;
        var stream = new MemoryStream();
        await file.OpenReadStream(GlobalVariable.MaxAllowedSize).CopyToAsync(stream);
        var command = new ImportProductsCommand(file.Name, stream.ToArray());

        var result = await Mediator.Send(command);
        if (result.Succeeded)
        {
            await _table.ReloadServerData();
            Snackbar.Add($"{ConstantString.ImportSuccess}", Severity.Info);
        }
        else
        {
            foreach (var msg in result.Errors)
            {
                Snackbar.Add($"{msg}", Severity.Error);
            }
        }
        _uploading = false;
    }

}