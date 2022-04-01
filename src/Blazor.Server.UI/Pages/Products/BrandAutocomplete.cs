using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Pages.Products;

public class BrandAutocomplete : MudAutocomplete<string>
{
    [Inject]
    private IStringLocalizer<BrandAutocomplete> L { get; set; } = default!;
    [Inject]
    private ISender _mediator { get; set; } = default!;

    private List<KeyValueDto> _brands = new();
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Dense = true;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchBrands;
        ToStringFunc = GetBrandName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _brands.Count==0)
        {
            var result = await _mediator.Send(new KeyValuesQueryByName("Brand"));
            _brands = result.ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<string>> SearchBrands(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(_brands.Select(x => x.Value ?? String.Empty));
        return Task.FromResult(_brands.Where(x => x.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase)
                                                 || x.Text.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Value ?? String.Empty));
    }

    private string GetBrandName(string value) => _brands.Find(b => b.Value == value)?.Text ?? String.Empty;
}
