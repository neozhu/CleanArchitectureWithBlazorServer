using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class PickUserAutocomplete : MudAutocomplete<string>
{
    private List<ApplicationUserDto>? _userList;

    [Parameter] public string? TenantId { get; set; }

    [Inject] private IUserService UserService { get; set; } = default!;

    protected override void OnInitialized()
    {
        UserService.OnChange += TenantsService_OnChange;
    }

    private void TenantsService_OnChange()
    {
        InvokeAsync(StateHasChanged);
    }

    protected override void Dispose(bool disposing)
    {
        UserService.OnChange -= TenantsService_OnChange;
        base.Dispose(disposing);
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFuncWithCancel = SearchKeyValues;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        MaxItems = 50;
        _userList = string.IsNullOrEmpty(TenantId)
            ? UserService.DataSource
            : UserService.DataSource.Where(x => x.TenantId == TenantId).ToList();
        return base.SetParametersAsync(parameters);
    }

    private Task<IEnumerable<string>> SearchKeyValues(string value, CancellationToken cancellation)
    {
        var result = string.IsNullOrEmpty(value)
            ? _userList?.Select(x => x.UserName)
            : _userList?.Where(x => x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                    x.Email.Contains(value, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.UserName);

        return Task.FromResult(result?.AsEnumerable() ?? new string[] { });
    }

    private string ToString(string str)
    {
        var user = _userList?.FirstOrDefault(x =>
            (x.DisplayName != null && x.DisplayName.Contains(str, StringComparison.OrdinalIgnoreCase)) ||
            x.UserName.Contains(str, StringComparison.OrdinalIgnoreCase));

        return user?.DisplayName ?? str;
    }
}