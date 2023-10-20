using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.Caching;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.PaginationQuery;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.Specifications;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Pages.SystemManagement;
public partial class AuditTrails
{
    public string Title { get; private set; } = "Audit Trails";
    private MudDataGrid<AuditTrailDto> _table = null!;
    private bool _loading;
    private int _defaultPageSize = 15;
    [Inject]
    private IState<UserProfileState> UserProfileState { get; set; } = null!;
    private UserProfile UserProfile => UserProfileState.Value.UserProfile;

    [Inject]
    private IMediator Mediator { get; set; } = default!;
    private readonly AuditTrailDto _currentDto = new();
    private AuditTrailsWithPaginationQuery Query { get; set; } = new();

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        Title = L[_currentDto.GetClassDescription()];
        var state = await AuthState;
    }

    private async Task<GridData<AuditTrailDto>> ServerReload(GridState<AuditTrailDto> state)
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
            return new GridData<AuditTrailDto>() { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }
    private async Task OnChangedListView(AuditTrailListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }
    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await _table.ReloadServerData();
    }
    private async Task OnSearch(AuditType? val)
    {
        Query.AuditType = val;
        await _table.ReloadServerData();
    }
    private async Task OnRefresh()
    {
        AuditTrailsCacheKey.Refresh();
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }
    private Task OnShowDetail(AuditTrailDto dto)
    {
        dto.ShowDetails = !dto.ShowDetails;
        return Task.CompletedTask;
    }
}