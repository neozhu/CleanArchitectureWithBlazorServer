using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

namespace Blazor.Server.UI.Components.Common;

public class MultiTenantAutocomplete : MudAutocomplete<string>
{
    [Inject]
    private ITenantService _tenantsService { get; set; } = default!;
    protected override void OnInitialized()
    {
        _tenantsService.OnChange += tenantsService_OnChange;
    }

    private void tenantsService_OnChange()
    {
       InvokeAsync(() =>StateHasChanged());
    }

    protected override void Dispose(bool disposing)
    {
        _tenantsService.OnChange -= tenantsService_OnChange;
        base.Dispose(disposing);
    }


    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFuncWithCancel = searchKeyValues;
        ToStringFunc = toStringFunc;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        return base.SetParametersAsync(parameters);
    }
    private Task<IEnumerable<string>> searchKeyValues(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            var result = _tenantsService.DataSource.OrderBy(x => x.Name).Select(x=>x.Id).ToList();
            return Task.FromResult<IEnumerable<string>>(result);
        }
        return Task.FromResult<IEnumerable<string>>(_tenantsService.DataSource.Where(x => x.Name!.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
          x.Description != null && x.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ).OrderBy(x => x.Name).Select(x=>x.Id).ToList());
                                         
    }
    private string toStringFunc(string val)
    {
        return _tenantsService.DataSource.Where(x => x.Id == val).Select(x => x.Name).FirstOrDefault()??"";
    }


}



