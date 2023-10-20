using CleanArchitecture.Blazor.Application.Features.Loggers.Commands.Clear;
using CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Loggers.Queries.ChatData;
using CleanArchitecture.Blazor.Application.Features.Loggers.Queries.PaginationQuery;
using CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
using CleanArchitecture.Blazor.Infrastructure.Constants.Permission;
using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Server.UI.Pages.SystemManagement.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Pages.SystemManagement;
public partial class Logs
{
    private LogsLineCharts? _chartComponent;
    private string Title { get; set; } = "Logs";
    private string _searchString = string.Empty;
    private MudDataGrid<LogDto> _table = default!;
    private readonly LogDto _currentDto = new();
    private int _defaultPageSize = 15;
    private List<LogTimeLineDto> Data { get; set; } = new();

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    private bool _loading;
    private bool _clearing;

    [Inject]
    private IMediator Mediator { get; set; } = default!;

    private bool _canPurge;
    private LogsWithPaginationQuery Query { get; } = new();

    protected override async void OnInitialized()
    {
        Title = L["Logs"];
        var state = await AuthState;
        _canPurge = (await AuthService.AuthorizeAsync(state.User, Permissions.Logs.Purge)).Succeeded;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Data = await Mediator.Send(new LogsTimeLineChatDataQuery());
            StateHasChanged();
            await _chartComponent!.RenderChart();
        }
    }

    private async Task<GridData<LogDto>> ServerReload(GridState<LogDto> state)
    {
        try
        {
            _loading = true;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;
            var result = await Mediator.Send(Query).ConfigureAwait(false);

            return new GridData<LogDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnChangedListView(LogListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task OnChangedLevel(LogLevel? level)
    {
        Query.Level = level;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }

    private async Task OnPurge()
    {
        var parameters = new DialogParameters<ConfirmationDialog>
    {
        { x=>x.ContentText, $"{L["Are you sure you want to erase all the logs?"]}" }
    };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<ConfirmationDialog>
            ($"{L["Erase logs"]}", parameters, options);
        var state = await dialog.Result;
        if (!state.Canceled)
        {
            _clearing = true;
            try
            {
                var command = new ClearLogsCommand();
                var result = await Mediator.Send(command);
                await _table.ReloadServerData();
                Snackbar.Add(L["Logs have been erased"], Severity.Info);
            }
            finally
            {
                _clearing = false;
            }
        }
    }

}