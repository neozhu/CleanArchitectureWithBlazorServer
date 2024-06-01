using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class MultiTenantAutocomplete : MudAutocomplete<string>
{
    [Inject] private ITenantService TenantsService { get; set; } = default!;

    protected override void OnInitialized()
    {
        TenantsService.OnChange += TenantsService_OnChange;
    }
    public MultiTenantAutocomplete()
    {
        SearchFunc = SearchKeyValues;
        ToStringFunc = ToTenantNameStringFunc;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
    }
    private void TenantsService_OnChange()
    {
        InvokeAsync(StateHasChanged);
    }

    protected override void Dispose(bool disposing)
    {
        TenantsService.OnChange -= TenantsService_OnChange;
        base.Dispose(disposing);
    }

    private Task<IEnumerable<string>> SearchKeyValues(string value, CancellationToken cancellation)
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            var result = TenantsService.DataSource.OrderBy(x => x.Name).Select(x => x.Id).ToList();
            return Task.FromResult<IEnumerable<string>>(result);
        }

        return Task.FromResult<IEnumerable<string>>(TenantsService.DataSource.Where(x =>
            x.Name!.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
            (x.Description != null && x.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        ).OrderBy(x => x.Name).Select(x => x.Id).ToList());
    }

    private string ToTenantNameStringFunc(string val)
    {
        return TenantsService.DataSource.Where(x => x.Id == val).Select(x => x.Name).FirstOrDefault() ?? "";
    }
}