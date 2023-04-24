using CleanArchitecture.Blazor.Domain.Enums;

namespace Blazor.Server.UI.Components.Common;

public class PicklistAutocomplete : MudAutocomplete<string>
{
    [Parameter]
    public Picklist Picklist { get; set; }

    [Inject]
    private IPicklistService PicklistService { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        PicklistService.OnChange += _picklistService_OnChange;
        await base.OnInitializedAsync();
    }

    private void _picklistService_OnChange()
    {
       InvokeAsync(() => StateHasChanged());
    }

    protected override void Dispose(bool disposing)
    {
        PicklistService.OnChange -= _picklistService_OnChange;
        base.Dispose(disposing);
    }
    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFunc = SearchKeyValues;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        return base.SetParametersAsync(parameters);
    }
    private Task<IEnumerable<string>> SearchKeyValues(string value)
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(PicklistService.DataSource.Where(x => x.Name == Picklist).Select(x => x.Value ?? String.Empty));
        }
        return Task.FromResult(PicklistService.DataSource.Where(x => x.Name == Picklist &&
        ( x.Value!=null &&  x.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
          x.Text!=null && x.Text.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        )).Select(x=>x.Value??String.Empty));
    }
    
}



