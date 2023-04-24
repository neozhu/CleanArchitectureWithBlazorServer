using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

namespace Blazor.Server.UI.Components.Common;

public class MultiTenantAutocomplete : MudAutocomplete<string>
{
    [Inject]
    private ITenantService TenantsService { get; set; } = default!;
    protected override void OnInitialized()
    {
        TenantsService.OnChange += tenantsService_OnChange;
    }

    private void tenantsService_OnChange()
    {
       InvokeAsync(() => StateHasChanged());
    }

    protected override void Dispose(bool disposing)
    {
        TenantsService.OnChange -= tenantsService_OnChange;
        base.Dispose(disposing);
    }


    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFuncWithCancel = SearchKeyValues;
        base.ToStringFunc = ToStringFunc;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
        return base.SetParametersAsync(parameters);
    }
    private Task<IEnumerable<string>> SearchKeyValues(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            var result = TenantsService.DataSource.OrderBy(x => x.Name).Select(x=>x.Id).ToList();
            return Task.FromResult<IEnumerable<string>>(result);
        }
        return Task.FromResult<IEnumerable<string>>(TenantsService.DataSource.Where(x => x.Name!.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
          x.Description != null && x.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ).OrderBy(x => x.Name).Select(x=>x.Id).ToList());
                                         
    }
    private string ToStringFunc(string val)
    {
        return TenantsService.DataSource.Where(x => x.Id == val).Select(x => x.Name).FirstOrDefault()??"";
    }


}



