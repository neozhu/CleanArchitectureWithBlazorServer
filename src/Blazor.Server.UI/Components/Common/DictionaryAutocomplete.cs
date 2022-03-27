using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Blazor.Server.UI.Components.Common;

public class DictionaryAutocomplete:MudAutocomplete<string>
{
   
    [EditorRequired]
    [Parameter]
    public string Dictionary { get; set; } = default!;
    [EditorRequired]
    [Parameter]
    public IEnumerable<KeyValueDto> DataSource { get; set; } = default!;
    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFunc = SearchKeyValues;
        //ToStringFunc = GetText;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        CoerceText = true;
        return base.SetParametersAsync(parameters);
    }
    private Task<IEnumerable<string>> SearchKeyValues(string value)
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(DataSource.Where(x => x.Name == Dictionary).Select(x => x.Value ?? String.Empty));
        }
        return Task.FromResult(DataSource.Where(x => x.Name == Dictionary && ( x.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase)
                                                                            || x.Text.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                                                 )
                                         .Select(x=>x.Value??String.Empty));
    }
    //private string GetText(string value) => DataSource.FirstOrDefault(x => x.Name == Dictionary && x.Value == value)?.Text ?? String.Empty;
}
