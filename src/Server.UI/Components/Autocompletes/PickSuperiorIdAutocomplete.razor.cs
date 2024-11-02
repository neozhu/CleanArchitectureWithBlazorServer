using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class PickSuperiorIdAutocomplete<T> : MudAutocomplete<ApplicationUserDto>
{
 
    [Parameter] public string? TenantId { get; set; }
    [Parameter] public string? OwnerName { get; set; }
    [Inject] private IUserService UserService { get; set; } = default!;
    public PickSuperiorIdAutocomplete()
    {
  
        SearchFunc = SearchKeyValues;
        ToStringFunc = dto=>dto?.UserName;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        MaxItems = 200;
    }
    private Task<IEnumerable<ApplicationUserDto>> SearchKeyValues(string value, CancellationToken cancellation)
    {
        IEnumerable<ApplicationUserDto> result= UserService.DataSource.Where(x => (x.TenantId!=null && x.TenantId.Equals(TenantId)) &&   !x.UserName.Equals(OwnerName));
        if (!string.IsNullOrWhiteSpace(value))
        {
            result = UserService.DataSource.Where(x => x.TenantId.Equals(TenantId) && !x.UserName.Equals(OwnerName) && (x.UserName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                     x.Email.Contains(value, StringComparison.OrdinalIgnoreCase)));
        }
        return Task.FromResult(result);
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