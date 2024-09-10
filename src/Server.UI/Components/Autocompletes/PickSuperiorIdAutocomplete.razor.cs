using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class PickSuperiorIdAutocomplete : MudAutocomplete<string>
{
    private List<ApplicationUserDto>? _userList;
    [Parameter] public string? TenantId { get; set; }
    [Parameter] public string? OwnerName { get; set; }

    [Inject] private IUserService UserService { get; set; } = default!;
    public PickSuperiorIdAutocomplete()
    {
        SearchFunc = SearchKeyValues;
        ToStringFunc = ConvertIdToUserName;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        MaxItems = 200;
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadUserDataAsync();
    }

    private Task LoadUserDataAsync()
    {
        _userList =  UserService.DataSource
            .Where(x => TenantId == null || x.TenantId == TenantId)
            .ToList();
        return Task.CompletedTask;
    }

    private Task<IEnumerable<string>> SearchKeyValues(string value, CancellationToken cancellation)
    {
        if (_userList == null)
            return Task.FromResult<IEnumerable<string>>(new List<string>());

        var query = _userList.AsQueryable();

        if (!string.IsNullOrWhiteSpace(value))
        {
            query = query.Where(x => x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                     x.Email.Contains(value, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(OwnerName))
        {
            query = query.Where(x => !x.UserName.Equals(OwnerName, StringComparison.OrdinalIgnoreCase));
        }

        var result = query.Select(x => x.Id).Take(MaxItems ?? 50).ToList();

        return Task.FromResult(result.AsEnumerable());
    }

    private string ConvertIdToUserName(string id)
    {
        return _userList?.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase))?.UserName ?? string.Empty;
    }
    protected override void OnInitialized()
    {
        UserService.OnChange += TenantsService_OnChange;
    }

    private async Task TenantsService_OnChange()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected override void Dispose(bool disposing)
    {
        UserService.OnChange -= TenantsService_OnChange;
        base.Dispose(disposing);
    }
}