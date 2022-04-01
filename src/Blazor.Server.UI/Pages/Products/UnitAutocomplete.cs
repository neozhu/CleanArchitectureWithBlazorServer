using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Products;

public class UnitAutocomplete : MudAutocomplete<string>
{
    [Inject]
    private IStringLocalizer<UnitAutocomplete> L { get; set; } = default!;
    [Inject]
    private ISender _mediator { get; set; } = default!;

    private List<KeyValueDto> _units= new();
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Dense = true;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchUnits;
        ToStringFunc = GetUnitName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _units.Count==0)
        {
            var result = await _mediator.Send(new KeyValuesQueryByName("Unit"));
            _units = result.ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<string>> SearchUnits(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(_units.Select(x => x.Value ?? String.Empty));
        return Task.FromResult(_units.Where(x => x.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase)
                                                 || x.Text.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Value ?? String.Empty));
    }

    private string GetUnitName(string value) => _units.Find(b => b.Value == value)?.Text ?? String.Empty;
}
