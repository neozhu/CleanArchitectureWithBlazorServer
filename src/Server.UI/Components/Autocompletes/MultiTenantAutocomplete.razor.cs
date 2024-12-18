using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class MultiTenantAutocomplete<T> : MudAutocomplete<TenantDto>
{
    public MultiTenantAutocomplete()
    {
        SearchFunc = SearchKeyValues;
        ToStringFunc = dto => dto?.Name;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
    }

    [Inject] private ITenantService TenantsService { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        TenantsService.OnChange += TenantsService_OnChange;
    }

    private async Task TenantsService_OnChange()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        TenantsService.OnChange -= TenantsService_OnChange;
        await base.DisposeAsyncCore();
    }

    private Task<IEnumerable<TenantDto>> SearchKeyValues(string? value, CancellationToken cancellation)
    {
        IEnumerable<TenantDto> result;

        if (string.IsNullOrWhiteSpace(value))
            result = TenantsService.DataSource.ToList();
        else
            result = TenantsService.DataSource
                .Where(x => x.Name?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true ||
                            x.Description?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList();

        return Task.FromResult(result);
    }
}