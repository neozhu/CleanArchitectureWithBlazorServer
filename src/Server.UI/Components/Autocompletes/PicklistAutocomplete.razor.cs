using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class PicklistAutocomplete<T> : MudAutocomplete<string>
{
    public PicklistAutocomplete()
    {
        SearchFunc = SearchFunc_;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ToStringFunc = x =>
        {
            if (x != null && PicklistService != null)
                return PicklistService.DataSource.FirstOrDefault(y => y.Value != null &&
                                                                      y.Value.Equals(x))?.Text ?? x;
            return x;
        };
    }

    [Parameter] public Picklist Picklist { get; set; }

    [Inject] private IPicklistService PicklistService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        PicklistService.OnChange += PicklistService_OnChange;
        await base.OnInitializedAsync();
    }

    private async Task PicklistService_OnChange()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        PicklistService.OnChange -= PicklistService_OnChange;
        await base.DisposeAsyncCore();
    }

    private Task<IEnumerable<string>> SearchFunc_(string value, CancellationToken cancellation = default)
    {
        // if text is null or empty, show complete list
        return string.IsNullOrEmpty(value)
            ? Task.FromResult(PicklistService.DataSource
                .Where(x => x.Name == Picklist)
                .Select(x => x.Value ?? string.Empty))
            : Task.FromResult(PicklistService.DataSource
                .Where(x => x.Name == Picklist && Contains(x, value))
                .Select(x => x.Value ?? string.Empty));
    }

    private static bool Contains(PicklistSetDto model, string value)
    {
        return (model.Value != null && model.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase))
               || (model.Text != null && model.Text.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }
}