using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class PickUserAutocomplete : MudAutocomplete<string>
{
 
    public PickUserAutocomplete()
    {
        SearchFunc = SearchKeyValues;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        MaxItems = 50;
    }
    [Parameter] public string? TenantId { get; set; }

    [Inject] private IUserService UserService { get; set; } = default!;

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
    
   

    private Task<IEnumerable<string>> SearchKeyValues(string value,CancellationToken cancellation)
    {
        var result = string.IsNullOrEmpty(value)
            ? UserService.DataSource.Where(x=>!string.IsNullOrEmpty(TenantId) && x.TenantId==TenantId || string.IsNullOrEmpty(TenantId)).Select(x => x.UserName)
            : UserService.DataSource.Where(x => (!string.IsNullOrEmpty(TenantId) && x.TenantId == TenantId || string.IsNullOrEmpty(TenantId)) && 
                        (x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                    x.Email.Contains(value, StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.UserName);

        return Task.FromResult(result?.AsEnumerable() ?? new string[] { });
    }

    private string ToString(string str)
    {
        var user = UserService.DataSource.FirstOrDefault(x =>
            (x.DisplayName != null && x.DisplayName.Contains(str, StringComparison.OrdinalIgnoreCase)) ||
            x.UserName.Contains(str, StringComparison.OrdinalIgnoreCase));

        return user?.DisplayName ?? str;
    }
}