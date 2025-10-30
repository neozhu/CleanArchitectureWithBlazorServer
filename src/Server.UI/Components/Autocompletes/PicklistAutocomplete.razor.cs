using System.Linq.Expressions;
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
        
    }

    [Parameter] public Picklist Picklist { get; set; }

    [Inject] private IDataSourceService<PicklistSetDto> PicklistService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        PicklistService.OnChange += PicklistService_OnChange;
        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await PicklistService.InitializeAsync();
        }
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

    private async Task<IEnumerable<string>> SearchFunc_(string? value,  CancellationToken cancellation = default)
    {
        // if text is null or empty, show complete list
        var term = value?.Trim();

        Expression<Func<PicklistSetDto, bool>> predicate;

        if (string.IsNullOrEmpty(term))
        {
            predicate = x => x.Name == Picklist;
        }
        else
        {
            predicate = x =>
                x.Name == Picklist &&
                (
                    (x.Value != null && x.Value.Contains(term)) ||
                    (x.Text  != null && x.Text.Contains(term))
                );
        }

        var limit = MaxItems > 0 ? MaxItems : null;
        var results = await PicklistService.SearchAsync(predicate, limit, cancellation);
        return results.Select(x => x.Value ?? string.Empty);
    }

    
}
