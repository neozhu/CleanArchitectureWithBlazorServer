using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Components.Common;

public class DictionaryAutocomplete:MudAutocomplete<string>
{
    [Inject]
    private ISender _mediator { get; set; } = default!;
    private List<KeyValueDto> _keyvalues = new();
    [EditorRequired]
    [Parameter]
    public string Dictionary { get; set; } = default!;
    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFunc = SearchKeyValues;
        ToStringFunc = GetText;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var result = await _mediator.Send(new KeyValuesQueryByName(Dictionary));
            _keyvalues = result.ToList();
            ForceRender(true);
        }
    }

    private Task<IEnumerable<string>> SearchKeyValues(string value)
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(_keyvalues.Select(x=>x.Value??String.Empty));
        return Task.FromResult(_keyvalues.Where(x =>x.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.Text.Contains(value, StringComparison.InvariantCultureIgnoreCase)).Select(x=>x.Value??String.Empty));
    }
    private string GetText(string value) => _keyvalues.Find(b => b.Value == value)?.Text ?? String.Empty;
}
